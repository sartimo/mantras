using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Yugen.Domain.Common.Commands;
using Yugen.Domain.Common.Enums;
using Yugen.Domain.Containers;
using Yugen.Domain.Containers.Commands;
using Yugen.Domain.UserConfigs.Commands;
using Yugen.Domain.Windows;
using Yugen.Domain.Windows.Commands;
using Yugen.Domain.Workspaces.Commands;
using Yugen.Infrastructure.Bussing;
using Yugen.Infrastructure.Common.Commands;
using Yugen.Infrastructure.Exceptions;
using Yugen.Infrastructure.Utils;
using Yugen.Infrastructure.WindowsApi;

namespace Yugen.Domain.UserConfigs
{
  public class CommandParsingService
  {
    private readonly UserConfigService _userConfigService;

    public CommandParsingService(UserConfigService userConfigService)
    {
      _userConfigService = userConfigService;
    }

    public static string FormatCommand(string commandString)
    {
      // Remove leading/trailing whitespace (if present).
      commandString = commandString.Trim();

      // Remove leading/trailing single quotes.
      if (commandString.StartsWith("'") && commandString.EndsWith("'"))
        commandString = commandString.Trim('\'');

      // Remove leading/trailing double quotes.
      if (commandString.StartsWith("\"") && commandString.EndsWith("\""))
        commandString = commandString.Trim('"');

      var caseSensitiveCommandRegex = new List<Regex>
      {
        new Regex("^(exec).*", RegexOptions.IgnoreCase),
      };

      // Some commands are partially case-sensitive (eg. `exec ...`). To handle such
      // cases, only format part of the command string to be lowercase.
      foreach (var regex in caseSensitiveCommandRegex)
      {
        if (regex.IsMatch(commandString))
        {
          return regex.Replace(commandString, (Match match) =>
            match.Value.ToLowerInvariant()
          );
        }
      }

      return commandString.ToLowerInvariant();
    }

    public void ValidateCommand(string commandString)
    {
      ParseCommand(commandString, null);
    }

    public Command ParseCommand(string commandString, Container subjectContainer)
    {
      try
      {
        var commandParts = commandString.Split(" ");

        return commandParts[0] switch
        {
          "tiling" => ParseTilingCommand(commandParts, subjectContainer),
          // TODO: "layout <LAYOUT>" commands are deprecated. Remove in next major release.
          "layout" => ParseLayoutCommand(commandParts, subjectContainer),
          "focus" => ParseFocusCommand(commandParts),
          "move" => ParseMoveCommand(commandParts, subjectContainer),
          "resize" => ParseResizeCommand(commandParts, subjectContainer),
          "set" => ParseSetCommand(commandParts, subjectContainer),
          "toggle" => ParseToggleCommand(commandParts, subjectContainer),
          "exit" => ParseExitCommand(commandParts),
          "close" => subjectContainer is Window
            ? new CloseWindowCommand(subjectContainer as Window)
            : new NoopCommand(),
          "reload" => ParseReloadCommand(commandParts),
          "exec" => new ExecProcessCommand(
            ExtractProcessName(string.Join(" ", commandParts[1..])),
            ExtractProcessArgs(string.Join(" ", commandParts[1..]))
          ),
          "ignore" => subjectContainer is Window
            ? new IgnoreWindowCommand(subjectContainer as Window)
            : new NoopCommand(),
          "binding" => ParseBindingCommand(commandParts),
          _ => throw new ArgumentException(null, nameof(commandString)),
        };
      }
      catch
      {
        throw new FatalUserException($"Invalid command '{commandString}'.");
      }
    }

    private static Command ParseTilingCommand(string[] commandParts, Container subjectContainer)
    {
      return commandParts[1] switch
      {
        "direction" => commandParts[2] switch
        {
          "vertical" => new ChangeTilingDirectionCommand(subjectContainer, TilingDirection.Vertical),
          "horizontal" => new ChangeTilingDirectionCommand(subjectContainer, TilingDirection.Horizontal),
          "toggle" => new ToggleTilingDirectionCommand(subjectContainer),
          _ => throw new ArgumentException(null, nameof(commandParts)),
        },
        _ => throw new ArgumentException(null, nameof(commandParts)),
      };
    }

    private static Command ParseLayoutCommand(string[] commandParts, Container subjectContainer)
    {
      return commandParts[1] switch
      {
        "vertical" => new ChangeTilingDirectionCommand(subjectContainer, TilingDirection.Vertical),
        "horizontal" => new ChangeTilingDirectionCommand(subjectContainer, TilingDirection.Horizontal),
        _ => throw new ArgumentException(null, nameof(commandParts)),
      };
    }

    private Command ParseFocusCommand(string[] commandParts)
    {
      return commandParts[1] switch
      {
        "left" => new FocusInDirectionCommand(Direction.Left),
        "right" => new FocusInDirectionCommand(Direction.Right),
        "up" => new FocusInDirectionCommand(Direction.Up),
        "down" => new FocusInDirectionCommand(Direction.Down),
        "workspace" => ParseFocusWorkspaceCommand(commandParts),
        "mode" => commandParts[2] switch
        {
          "toggle" => new ToggleFocusModeCommand(),
          _ => throw new ArgumentException(null, nameof(commandParts)),
        },
        _ => throw new ArgumentException(null, nameof(commandParts)),
      };
    }

    private Command ParseFocusWorkspaceCommand(string[] commandParts)
    {
      return commandParts[2] switch
      {
        "recent" => new FocusWorkspaceRecentCommand(),
        "prev" => new FocusWorkspaceSequenceCommand(Sequence.Previous),
        "next" => new FocusWorkspaceSequenceCommand(Sequence.Next),
        _ when IsValidWorkspace(commandParts[2]) => new FocusWorkspaceCommand(commandParts[2]),
        _ => throw new ArgumentException(null, nameof(commandParts)),
      };
    }

    private Command ParseMoveCommand(string[] commandParts, Container subjectContainer)
    {
      return commandParts[1] switch
      {
        "left" => subjectContainer is Window
          ? new MoveWindowCommand(subjectContainer as Window, Direction.Left)
          : new NoopCommand(),
        "right" => subjectContainer is Window
          ? new MoveWindowCommand(subjectContainer as Window, Direction.Right)
          : new NoopCommand(),
        "up" => subjectContainer is Window
          ? new MoveWindowCommand(subjectContainer as Window, Direction.Up)
          : new NoopCommand(),
        "down" => subjectContainer is Window
          ? new MoveWindowCommand(subjectContainer as Window, Direction.Down)
          : new NoopCommand(),
        "to" when IsValidWorkspace(commandParts[3]) => subjectContainer is Window
          ? new MoveWindowToWorkspaceCommand(subjectContainer as Window, commandParts[3])
          : new NoopCommand(),
        "workspace" => ParseMoveWorkspaceCommand(commandParts),
        _ => throw new ArgumentException(null, nameof(commandParts)),
      };
    }

    private static Command ParseMoveWorkspaceCommand(string[] commandParts)
    {
      return commandParts[2] switch
      {
        "left" => new MoveWorkspaceInDirectionCommand(Direction.Left),
        "right" => new MoveWorkspaceInDirectionCommand(Direction.Right),
        "up" => new MoveWorkspaceInDirectionCommand(Direction.Up),
        "down" => new MoveWorkspaceInDirectionCommand(Direction.Down),
        _ => throw new ArgumentException(null, nameof(commandParts)),
      };
    }

    private static Command ParseResizeCommand(string[] commandParts, Container subjectContainer)
    {
      return commandParts[1] switch
      {
        "height" => subjectContainer is Window
          ? new ResizeWindowCommand(subjectContainer as Window, ResizeDimension.Height, commandParts[2])
          : new NoopCommand(),
        "width" => subjectContainer is Window
          ? new ResizeWindowCommand(subjectContainer as Window, ResizeDimension.Width, commandParts[2])
          : new NoopCommand(),
        "borders" => subjectContainer is Window
          ? new ResizeWindowBordersCommand(
            subjectContainer as Window,
            ShorthandToRectDelta(string.Join(" ", commandParts[2..]))
          )
          : new NoopCommand(),
        _ => throw new ArgumentException(null, nameof(commandParts)),
      };
    }

    private static Command ParseSetCommand(string[] commandParts, Container subjectContainer)
    {
      return commandParts[1] switch
      {
        "floating" => subjectContainer is Window
          ? new SetFloatingCommand(subjectContainer as Window)
          : new NoopCommand(),
        "minimized" => subjectContainer is Window
          ? new SetMinimizedCommand(subjectContainer as Window)
          : new NoopCommand(),
        "maximized" => subjectContainer is Window
          ? new SetMaximizedCommand(subjectContainer as Window)
          : new NoopCommand(),
        "tiling" => subjectContainer is Window
          ? new SetTilingCommand(subjectContainer as Window)
          : new NoopCommand(),
        "width" => subjectContainer is Window
          ? new SetWindowSizeCommand(subjectContainer as Window, ResizeDimension.Width, commandParts[2])
          : new NoopCommand(),
        "height" => subjectContainer is Window
          ? new SetWindowSizeCommand(subjectContainer as Window, ResizeDimension.Height, commandParts[2])
          : new NoopCommand(),
        _ => throw new ArgumentException(null, nameof(commandParts)),
      };
    }

    private static Command ParseToggleCommand(string[] commandParts, Container subjectContainer)
    {
      return commandParts[1] switch
      {
        "floating" => subjectContainer is Window
          ? new ToggleFloatingCommand(subjectContainer as Window)
          : new NoopCommand(),
        "maximized" => subjectContainer is Window
          ? new ToggleMaximizedCommand(subjectContainer as Window)
          : new NoopCommand(),
        // TODO: "toggle focus mode" command is deprecated. Remove in next major release.
        "focus" => commandParts[2] switch
        {
          "mode" => new ToggleFocusModeCommand(),
          _ => throw new ArgumentException(null, nameof(commandParts)),
        },
        _ => throw new ArgumentException(null, nameof(commandParts)),
      };
    }

    private static Command ParseExitCommand(string[] commandParts)
    {
      return commandParts[1] switch
      {
        "wm" => new ExitApplicationCommand(false),
        _ => throw new ArgumentException(null, nameof(commandParts)),
      };
    }

    private Command ParseBindingCommand(string[] commandParts)
    {
      return commandParts[1] switch
      {
        "mode" when IsValidBindingMode(commandParts[2]) =>
          new SetBindingModeCommand(commandParts[2]),
        _ => throw new ArgumentException(null, nameof(commandParts)),
      };
    }

    private static Command ParseReloadCommand(string[] commandParts)
    {
      return commandParts[1] switch
      {
        "config" => new ReloadUserConfigCommand(),
        _ => throw new ArgumentException(null, nameof(commandParts)),
      };
    }

    /// <summary>
    /// Whether a workspace exists with the given name.
    /// </summary>
    private bool IsValidWorkspace(string workspaceName)
    {
      var workspaceConfig = _userConfigService.GetWorkspaceConfigByName(workspaceName);

      return workspaceConfig is not null;
    }

    /// <summary>
    /// Whether a binding mode exists with the given name.
    /// </summary>
    private bool IsValidBindingMode(string bindingModeName)
    {
      var bindingMode = _userConfigService.GetBindingModeByName(bindingModeName);

      return bindingMode is not null || bindingModeName == "none";
    }

    public static string ExtractProcessName(string processNameAndArgs)
    {
      var hasSingleQuotes = processNameAndArgs.StartsWith(
        "'",
        StringComparison.InvariantCulture
      );

      return hasSingleQuotes
        ? processNameAndArgs.Split("'")[1]
        : processNameAndArgs.Split(" ")[0];
    }

    public static List<string> ExtractProcessArgs(string processNameAndArgs)
    {
      var hasSingleQuotes = processNameAndArgs.StartsWith(
        "'",
        StringComparison.InvariantCulture
      );

      var args = hasSingleQuotes
        ? processNameAndArgs.Split("'")[2..]
        : processNameAndArgs.Split(" ")[1..];

      return args.Where(arg => !string.IsNullOrWhiteSpace(arg)).ToList();
    }

    public static RectDelta ShorthandToRectDelta(string shorthand)
    {
      var shorthandParts = shorthand.Split(" ")
        .Select(shorthandPart => UnitsHelper.TrimUnits(shorthandPart))
        .ToList();

      return shorthandParts.Count switch
      {
        1 => new RectDelta(shorthandParts[0], shorthandParts[0], shorthandParts[0], shorthandParts[0]),
        2 => new RectDelta(shorthandParts[1], shorthandParts[0], shorthandParts[1], shorthandParts[0]),
        3 => new RectDelta(shorthandParts[1], shorthandParts[0], shorthandParts[1], shorthandParts[2]),
        4 => new RectDelta(shorthandParts[3], shorthandParts[0], shorthandParts[1], shorthandParts[2]),
        _ => throw new ArgumentException(null, nameof(shorthand)),
      };
    }
  }
}
