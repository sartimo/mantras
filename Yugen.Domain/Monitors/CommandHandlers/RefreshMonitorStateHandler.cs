using System.Linq;
using System.Windows.Forms;
using Yugen.Domain.Containers;
using Yugen.Domain.Containers.Commands;
using Yugen.Domain.Monitors.Commands;
using Yugen.Domain.Monitors.Events;
using Yugen.Domain.Windows;
using Yugen.Domain.Workspaces;
using Yugen.Infrastructure.Bussing;

namespace Yugen.Domain.Monitors.CommandHandlers
{
  internal sealed class RefreshMonitorStateHandler : ICommandHandler<RefreshMonitorStateCommand>
  {
    private readonly Bus _bus;
    private readonly MonitorService _monitorService;
    private readonly ContainerService _containerService;
    private readonly WindowService _windowService;

    public RefreshMonitorStateHandler(
      Bus bus,
      MonitorService monitorService,
      ContainerService containerService,
      WindowService windowService)
    {
      _bus = bus;
      _monitorService = monitorService;
      _containerService = containerService;
      _windowService = windowService;
    }

    public CommandResponse Handle(RefreshMonitorStateCommand command)
    {
      foreach (var screen in Screen.AllScreens)
      {
        var foundMonitor = _monitorService.GetMonitorByDeviceName(screen.DeviceName);

        // Add monitor if it doesn't exist in state.
        if (foundMonitor == null)
        {
          _bus.Invoke(new AddMonitorCommand(screen));
          continue;
        }

        // Update monitor with changes to dimensions and positioning.
        foundMonitor.Width = screen.WorkingArea.Width;
        foundMonitor.Height = screen.WorkingArea.Height;
        foundMonitor.X = screen.WorkingArea.X;
        foundMonitor.Y = screen.WorkingArea.Y;

        _bus.Emit(new WorkingAreaResizedEvent(foundMonitor));
      }

      // Verify that all monitors in state exist in `Screen.AllScreens`.
      var monitorsToRemove = _monitorService.GetMonitors().Where(
        monitor => !IsMonitorActive(monitor)
      );

      // Remove any monitors that no longer exist and move their workspaces to other monitors.
      // TODO: Verify that this works with "Duplicate these displays" or "Show only X" settings.
      foreach (var monitor in monitorsToRemove.ToList())
      {
        _bus.Invoke(new RemoveMonitorCommand(monitor));
      }

      foreach (var window in _windowService.GetWindows())
      {
        // Display setting changes can spread windows out sporadically, so mark all windows as
        // needing a DPI adjustment (just in case).
        window.HasPendingDpiAdjustment = true;

        // Need to update floating position of moved windows when a monitor is disconnected or if
        // the primary display is changed. The primary display dictates the position of 0,0.
        var parentWorkspace = WorkspaceService.GetWorkspaceFromChildContainer(window);
        window.FloatingPlacement =
          window.FloatingPlacement.TranslateToCenter(parentWorkspace.ToRect());
      }

      // Redraw full container tree.
      _containerService.ContainersToRedraw.Add(_containerService.ContainerTree);

      return CommandResponse.Ok;
    }

    private static bool IsMonitorActive(Monitor monitor)
    {
      return Screen.AllScreens.Any(screen => screen.DeviceName == monitor.DeviceName);
    }
  }
}
