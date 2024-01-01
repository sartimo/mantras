using System.Linq;
using Yugen.Domain.Common.Utils;
using Yugen.Domain.Containers;
using Yugen.Domain.Containers.Commands;
using Yugen.Domain.Workspaces;
using Yugen.Infrastructure.Bussing;
using Yugen.Infrastructure.Common.Events;
using Microsoft.Extensions.Logging;

namespace Yugen.Domain.Windows.EventHandlers
{
  internal sealed class WindowMinimizedHandler : IEventHandler<WindowMinimizedEvent>
  {
    private readonly Bus _bus;
    private readonly WindowService _windowService;
    private readonly ContainerService _containerService;
    private readonly ILogger<WindowMinimizedHandler> _logger;

    public WindowMinimizedHandler(
      Bus bus,
      WindowService windowService,
      ContainerService containerService,
      ILogger<WindowMinimizedHandler> logger)
    {
      _bus = bus;
      _windowService = windowService;
      _containerService = containerService;
      _logger = logger;
    }

    public void Handle(WindowMinimizedEvent @event)
    {
      var window = _windowService.GetWindows()
        .FirstOrDefault(window => window.Handle == @event.WindowHandle);

      if (window is null or MinimizedWindow)
        return;

      _logger.LogWindowEvent("Window minimized", window);

      var workspace = WorkspaceService.GetWorkspaceFromChildContainer(window);

      // Move tiling windows to be direct children of workspace (in case they aren't already).
      if (window is TilingWindow)
        _bus.Invoke(new MoveContainerWithinTreeCommand(window, workspace));

      var previousState = WindowService.GetWindowType(window);
      var minimizedWindow = new MinimizedWindow(
        window.Handle,
        window.FloatingPlacement,
        window.BorderDelta,
        previousState
      )
      {
        Id = window.Id
      };

      // Get container to switch focus to after the window has been minimized. Need to
      // get focus target prior to calling `ReplaceContainerCommand`.
      var focusedContainer = _containerService.FocusedContainer;
      var focusTarget = window == focusedContainer
        ? WindowService.GetFocusTargetAfterRemoval(window)
        : null;

      _bus.Invoke(new ReplaceContainerCommand(minimizedWindow, window.Parent, window.Index));

      // Focus should be reassigned to appropriate container.
      if (focusTarget is not null)
      {
        _bus.Invoke(new SetFocusedDescendantCommand(focusTarget));
        _containerService.HasPendingFocusSync = true;
        _windowService.UnmanagedOrMinimizedStopwatch.Restart();
      }

      _containerService.ContainersToRedraw.Add(workspace);
      _bus.Invoke(new RedrawContainersCommand());
      _bus.Invoke(new SyncNativeFocusCommand());
    }
  }
}
