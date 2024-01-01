using System;
using Yugen.Domain.Containers.Commands;
using Yugen.Domain.Containers.Events;
using Yugen.Domain.Windows;
using Yugen.Domain.Workspaces;
using Yugen.Infrastructure.Bussing;
using static Yugen.Infrastructure.WindowsApi.WindowsApiService;

namespace Yugen.Domain.Containers.CommandHandlers
{
  internal sealed class SyncNativeFocusHandler : ICommandHandler<SyncNativeFocusCommand>
  {
    private readonly Bus _bus;
    private readonly ContainerService _containerService;

    public SyncNativeFocusHandler(Bus bus, ContainerService containerService)
    {
      _bus = bus;
      _containerService = containerService;
    }

    public CommandResponse Handle(SyncNativeFocusCommand command)
    {
      var hasPendingFocusSync = _containerService.HasPendingFocusSync;

      if (!hasPendingFocusSync)
        return CommandResponse.Ok;

      // Container that the WM believes should have focus.
      var focusedContainer = _containerService.FocusedContainer;

      var handleToFocus = focusedContainer switch
      {
        Window window => window.Handle,
        Workspace => GetDesktopWindow(),
        _ => throw new Exception("Invalid container type to focus. This is a bug."),
      };

      // Set focus to the given window handle. If the container is a normal window, then this
      // will trigger `EVENT_SYSTEM_FOREGROUND` window event and its handler.
      KeybdEvent(0, 0, 0, 0);
      SetForegroundWindow(handleToFocus);

      _bus.Emit(new NativeFocusSyncedEvent(focusedContainer));
      _bus.Emit(new FocusChangedEvent(focusedContainer));

      _containerService.HasPendingFocusSync = false;

      return CommandResponse.Ok;
    }
  }
}
