using System.Linq;
using Yugen.Domain.Common.Enums;
using Yugen.Domain.Common.Utils;
using Yugen.Domain.Containers;
using Yugen.Domain.Containers.Commands;
using Yugen.Domain.Containers.Events;
using Yugen.Domain.Monitors;
using Yugen.Domain.Windows.Commands;
using Yugen.Domain.Workspaces;
using Yugen.Infrastructure.Bussing;
using Yugen.Infrastructure.Common.Events;
using Yugen.Infrastructure.WindowsApi;
using Microsoft.Extensions.Logging;

namespace Yugen.Domain.Windows.EventHandlers
{
  internal sealed class WindowMovedOrResizedHandler : IEventHandler<WindowMovedOrResizedEvent>
  {
    private readonly Bus _bus;
    private readonly WindowService _windowService;
    private readonly MonitorService _monitorService;
    private readonly ContainerService _containerService;
    private readonly ILogger<WindowMovedOrResizedHandler> _logger;

    public WindowMovedOrResizedHandler(
      Bus bus,
      WindowService windowService,
      MonitorService monitorService,
      ILogger<WindowMovedOrResizedHandler> logger,
      ContainerService containerService)
    {
      _bus = bus;
      _windowService = windowService;
      _monitorService = monitorService;
      _logger = logger;
      _containerService = containerService;
    }

    public void Handle(WindowMovedOrResizedEvent @event)
    {
      var window = _windowService.GetWindows().FirstOrDefault(window => window.Handle == @event.WindowHandle);

      if (window is null)
        return;

      _logger.LogWindowEvent("Window moved/resized", window);

      if (window is TilingWindow)
      {
        UpdateTilingWindow(window as TilingWindow);
        return;
      }

      if (window is FloatingWindow)
        UpdateFloatingWindow(window as FloatingWindow);
    }

    private void UpdateTilingWindow(TilingWindow window)
    {
      // Snap window to its original position even if it's not being resized.
      var hasNoResizableSiblings = window.Parent is Workspace
      && !window.SiblingsOfType<IResizable>().Any();

      if (hasNoResizableSiblings)
      {
        _containerService.ContainersToRedraw.Add(window);
        _bus.Invoke(new RedrawContainersCommand());
        return;
      }

      // Remove invisible borders from current placement to be able to compare window width/height.
      var currentPlacement = WindowService.GetPlacementOfHandle(window.Handle).NormalPosition;
      var adjustedPlacement = new Rect
      {
        Left = currentPlacement.Left + window.BorderDelta.Left,
        Right = currentPlacement.Right - window.BorderDelta.Right,
        Top = currentPlacement.Top + window.BorderDelta.Top,
        Bottom = currentPlacement.Bottom - window.BorderDelta.Bottom,
      };

      var deltaWidth = adjustedPlacement.Width - window.Width;
      var deltaHeight = adjustedPlacement.Height - window.Height;

      _bus.Invoke(new ResizeWindowCommand(window, ResizeDimension.Width, $"{deltaWidth}px"));
      _bus.Invoke(new ResizeWindowCommand(window, ResizeDimension.Height, $"{deltaHeight}px"));
      _bus.Invoke(new RedrawContainersCommand());
    }

    private void UpdateFloatingWindow(FloatingWindow window)
    {
      // Update state with new location of the floating window.
      window.FloatingPlacement = WindowService.GetPlacementOfHandle(window.Handle).NormalPosition;

      // Change floating window's parent workspace if out of its bounds.
      UpdateParentWorkspace(window);
    }

    private void UpdateParentWorkspace(Window window)
    {
      var currentWorkspace = WorkspaceService.GetWorkspaceFromChildContainer(window);

      // Get workspace that encompasses most of the window.
      var targetMonitor = _monitorService.GetMonitorFromHandleLocation(window.Handle);
      var targetWorkspace = targetMonitor.DisplayedWorkspace;

      // Ignore if window is still within the bounds of its current workspace.
      if (currentWorkspace == targetWorkspace)
        return;

      // Change the window's parent workspace.
      _bus.Invoke(new MoveContainerWithinTreeCommand(window, targetWorkspace));
      _bus.Emit(new FocusChangedEvent(window));
    }
  }
}
