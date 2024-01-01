using System;
using System.Linq;
using System.Threading.Tasks;
using Yugen.Domain.Containers.Commands;
using Yugen.Domain.UserConfigs.Commands;
using Yugen.Domain.Windows;
using Yugen.Infrastructure.Bussing;
using Yugen.Infrastructure.Common.Commands;
using Yugen.Infrastructure.WindowsApi;
using static Yugen.Infrastructure.WindowsApi.WindowsApiService;

namespace Yugen.Domain.UserConfigs.CommandHandlers
{
  internal sealed class RegisterKeybindingsHandler : ICommandHandler<RegisterKeybindingsCommand>
  {
    private readonly Bus _bus;
    private readonly KeybindingService _keybindingService;
    private readonly WindowService _windowService;

    public RegisterKeybindingsHandler(
      Bus bus,
      KeybindingService keybindingService,
      WindowService windowService)
    {
      _bus = bus;
      _keybindingService = keybindingService;
      _windowService = windowService;
    }

    public CommandResponse Handle(RegisterKeybindingsCommand command)
    {
      _keybindingService.Reset();

      foreach (var keybindingConfig in command.Keybindings)
      {
        // Format command strings defined in keybinding config.
        var commandStrings = keybindingConfig.CommandList.Select(
          CommandParsingService.FormatCommand
        );

        // Register all keybindings for a command sequence.
        foreach (var binding in keybindingConfig.BindingList)
          _keybindingService.AddGlobalKeybinding(binding, () =>
          {
            Task.Run(() =>
            {
              try
              {
                lock (_bus.LockObj)
                {
                  // Avoid invoking keybinding if an ignored window currently has focus.
                  if (_windowService.IgnoredHandles.Contains(GetForegroundWindow()))
                    return;

                  _bus.Invoke(new RunWithSubjectContainerCommand(commandStrings));
                  _bus.Invoke(new RedrawContainersCommand());
                  _bus.Invoke(new SyncNativeFocusCommand());
                }
              }
              catch (Exception e)
              {
                _bus.Invoke(new HandleFatalExceptionCommand(e));
                throw;
              }
            });
          });
      }

      return CommandResponse.Ok;
    }
  }
}
