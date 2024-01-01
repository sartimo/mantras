using System.Linq;
using Yugen.Domain.Common.Enums;
using Yugen.Domain.Common.Utils;
using Yugen.Domain.Containers;
using Yugen.Domain.Containers.Commands;
using Yugen.Domain.Containers.Events;
using Yugen.Domain.Workspaces;
using Yugen.Domain.Workspaces.Commands;
using Yugen.Infrastructure.Bussing;
using Yugen.Infrastructure.Common.Events;
using Microsoft.Extensions.Logging;

namespace Yugen.Domain.Windows.EventHandlers
{
  internal sealed class WindowFocusedHandler : IEventHandler<WindowFocusedEvent>
  {
    private readonly Bus _bus;
    private readonly ContainerService _containerService;
    private readonly ILogger<WindowFocusedHandler> _logger;
    private readonly WindowService _windowService;

    public WindowFocusedHandler(
      Bus bus,
      ContainerService containerService,
      ILogger<WindowFocusedHandler> logger,
      WindowService windowService)
    {
      _bus = bus;
      _containerService = containerService;
      _logger = logger;
      _windowService = windowService;
    }

    public void Handle(WindowFocusedEvent @event)
    {
      var windowHandle = @event.WindowHandle;

      var window = _windowService.GetWindows()
        .FirstOrDefault(window => window.Handle == @event.WindowHandle);

      // Ignore event if window is unmanaged or being hidden by the WM.
      if (window is null || window.DisplayState is DisplayState.Hiding)
        return;

      _logger.LogWindowEvent("Native focus event", window);

      var focusedContainer = _containerService.FocusedContainer;

      // Focus is already set to the WM's focused container.
      if (window == focusedContainer)
        return;

      var unmanagedStopwatch = _windowService.UnmanagedOrMinimizedStopwatch;

      // Handle overriding focus on close/minimize. After a window is closed or minimized,
      // the OS or the closed application might automatically switch focus to a different
      // window. To force focus to go to the WM's target focus container, we reassign any
      // focus events 100ms after close/minimize. This will cause focus to briefly flicker
      // to the OS focus target and then to the WM's focus target.
      if (unmanagedStopwatch.IsRunning && unmanagedStopwatch.ElapsedMilliseconds < 100)
      {
        _logger.LogDebug("Overriding native focus.");
        _bus.Invoke(new SyncNativeFocusCommand());
        return;
      }

      // Handle focus events from windows on hidden workspaces. For example, if Discord
      // is forcefully shown by the OS when it's on a hidden workspace, switch focus to
      // Discord's workspace.
      if (window.DisplayState is DisplayState.Hidden)
      {
        _logger.LogWindowEvent("Focusing off-screen window", window);

        var workspace = WorkspaceService.GetWorkspaceFromChildContainer(window);
        _bus.Invoke(new FocusWorkspaceCommand(workspace.Name));
        _bus.Invoke(new SetFocusedDescendantCommand(window));
        _bus.Invoke(new RedrawContainersCommand());
        _bus.Emit(new FocusChangedEvent(window));
        return;
      }

      // Update the WM's focus state.
      _bus.Invoke(new SetFocusedDescendantCommand(window));
      _bus.Emit(new FocusChangedEvent(window));
    }
  }
}
