using System;
using System.Linq;
using Yugen.Domain.Common.Enums;
using Yugen.Domain.UserConfigs;
using Yugen.Domain.Workspaces.Commands;
using Yugen.Infrastructure.Bussing;

namespace Yugen.Domain.Workspaces.CommandHandlers
{
  internal sealed class FocusWorkspaceSequenceHandler : ICommandHandler<FocusWorkspaceSequenceCommand>
  {
    private readonly Bus _bus;
    private readonly UserConfigService _userConfigService;
    private readonly WorkspaceService _workspaceService;

    public FocusWorkspaceSequenceHandler(
      Bus bus,
      UserConfigService userConfigService,
      WorkspaceService workspaceService)
    {
      _bus = bus;
      _userConfigService = userConfigService;
      _workspaceService = workspaceService;
    }

    public CommandResponse Handle(FocusWorkspaceSequenceCommand command)
    {
      var direction = command.Direction;
      var workspacesConfigs = _userConfigService.WorkspaceConfigs;

      // Get active workspaces in order of their config index.
      var activeWorkspaces = _workspaceService.GetActiveWorkspaces();
      var sortedWorkspaces = activeWorkspaces
        .OrderBy((workspace) =>
          workspacesConfigs.FindIndex((config) => config.Name == workspace.Name)
        )
        .ToList();

      // Get config index of the currently focused workspace.
      var focusedWorkspace = _workspaceService.GetFocusedWorkspace();
      var configIndex = sortedWorkspaces.IndexOf(focusedWorkspace);

      // Get index in `sortedWorkspaces` of target workspace to focus. Wrap around to start if
      // there is no previous/next workspace.
      var indexToFocus = direction switch
      {
        Sequence.Previous => configIndex == 0 ? sortedWorkspaces.Count - 1 : configIndex - 1,
        Sequence.Next => configIndex == sortedWorkspaces.Count - 1 ? 0 : configIndex + 1,
        _ => throw new ArgumentException(nameof(direction)),
      };

      var workspaceToFocus = sortedWorkspaces.ElementAtOrDefault(indexToFocus);

      // Set focus to the previous/next workspace if found.
      if (workspaceToFocus is not null && workspaceToFocus != focusedWorkspace)
        _bus.Invoke(new FocusWorkspaceCommand(workspaceToFocus.Name));

      return CommandResponse.Ok;
    }
  }
}
