using System;
using System.Linq;
using Yugen.Domain.Containers;
using Yugen.Domain.Containers.Commands;
using Yugen.Domain.Monitors.Commands;
using Yugen.Domain.Workspaces;
using Yugen.Infrastructure.Bussing;
using Yugen.Infrastructure.Common.Events;
using static Yugen.Infrastructure.WindowsApi.WindowsApiService;

namespace Yugen.Domain.Windows.EventHandlers
{
  internal sealed class WindowLocationChangedHandler : IEventHandler<WindowLocationChangedEvent>
  {
    private readonly Bus _bus;
    private readonly ContainerService _containerService;
    private readonly WindowService _windowService;

    public WindowLocationChangedHandler(Bus bus,
      WindowService windowService,
      ContainerService containerService)
    {
      _bus = bus;
      _containerService = containerService;
      _windowService = windowService;
    }

    public void Handle(WindowLocationChangedEvent @event)
    {
      var windowHandle = @event.WindowHandle;

      HandleMaximizedWindow(windowHandle);

      if (!_windowService.AppBarHandles.Contains(windowHandle))
        return;

      _bus.Invoke(new RefreshMonitorStateCommand());
      _bus.Invoke(new RedrawContainersCommand());
    }

    private void HandleMaximizedWindow(IntPtr windowHandle)
    {
      var window = _windowService.GetWindowByHandle(windowHandle);
      if (window is null)
        return;

      var windowPlacement = WindowService.GetPlacementOfHandle(windowHandle);
      var isMaximized = windowPlacement.ShowCommand == ShowWindowFlags.Maximize;

      // Window is being maximized.
      if (isMaximized && window is not MaximizedWindow)
      {
        var previousState = WindowService.GetWindowType(window);
        var maximizedWindow = new MaximizedWindow(
          window.Handle,
          window.FloatingPlacement,
          window.BorderDelta,
          previousState
        )
        {
          Id = window.Id
        };

        if (!window.HasSiblings() && window.Parent is not Workspace)
          _bus.Invoke(new FlattenSplitContainerCommand(window.Parent as SplitContainer));

        _bus.Invoke(new ReplaceContainerCommand(maximizedWindow, window.Parent, window.Index));

        _containerService.ContainersToRedraw.Concat(window.Siblings);
        _bus.Invoke(new RedrawContainersCommand());
      }

      // Maximized window is being restored.
      if (!isMaximized && window is MaximizedWindow)
      {
        var restoredWindow = CreateWindowFromPreviousState(window as MaximizedWindow);
        _bus.Invoke(new ReplaceContainerCommand(restoredWindow, window.Parent, window.Index));

        // Non-tiling window expect to be direct children of workspace.
        if (restoredWindow is not TilingWindow)
        {
          var workspace = WorkspaceService.GetWorkspaceFromChildContainer(restoredWindow);
          _bus.Invoke(new MoveContainerWithinTreeCommand(restoredWindow, workspace));
        }
        else
        {
          // TODO: Temporary hack to resize restored tiling window.
          _bus.Invoke(
            new MoveContainerWithinTreeCommand(
              restoredWindow,
              restoredWindow.Parent,
              restoredWindow.Index
            )
          );
        }

        _containerService.ContainersToRedraw.Add(restoredWindow);
        _bus.Invoke(new RedrawContainersCommand());
      }
    }

    // TODO: Share logic with `WindowMinimizedHandler`.
    private static Window CreateWindowFromPreviousState(MaximizedWindow window)
    {
      Window restoredWindow = window.PreviousState switch
      {
        WindowType.Floating => new FloatingWindow(
          window.Handle,
          window.FloatingPlacement,
          window.BorderDelta
        ),
        WindowType.Fullscreen => new FullscreenWindow(
          window.Handle,
          window.FloatingPlacement,
          window.BorderDelta
        ),
        // Set `SizePercentage` to 0 to correctly resize the container when moved within tree.
        WindowType.Tiling => new TilingWindow(
          window.Handle,
          window.FloatingPlacement,
          window.BorderDelta,
          0
        ),
        WindowType.Minimized => new MinimizedWindow(
          window.Handle,
          window.FloatingPlacement,
          window.BorderDelta,
          WindowType.Tiling
        ),
        WindowType.Maximized => throw new ArgumentException(null, nameof(window)),
        _ => throw new ArgumentException(null, nameof(window)),
      };

      restoredWindow.Id = window.Id;
      return restoredWindow;
    }
  }
}
