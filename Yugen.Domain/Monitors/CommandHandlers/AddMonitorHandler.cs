using Yugen.Domain.Containers;
using Yugen.Domain.Containers.Commands;
using Yugen.Domain.Monitors.Commands;
using Yugen.Domain.Monitors.Events;
using Yugen.Domain.Workspaces;
using Yugen.Domain.Workspaces.Commands;
using Yugen.Infrastructure.Bussing;
using Yugen.Infrastructure.Exceptions;

namespace Yugen.Domain.Monitors.CommandHandlers
{
  internal sealed class AddMonitorHandler : ICommandHandler<AddMonitorCommand>
  {
    private readonly Bus _bus;
    private readonly ContainerService _containerService;
    private readonly WorkspaceService _workspaceService;

    public AddMonitorHandler(
      Bus bus,
      ContainerService containerService,
      WorkspaceService workspaceService)
    {
      _bus = bus;
      _containerService = containerService;
      _workspaceService = workspaceService;
    }

    public CommandResponse Handle(AddMonitorCommand command)
    {
      var screen = command.Screen;

      // Create a `Monitor` instance. Use the working area of the monitor instead of the bounds of
      // the display. The working area excludes taskbars and other reserved display space.
      var newMonitor = new Monitor(
        screen.DeviceName,
        screen.WorkingArea.Width,
        screen.WorkingArea.Height,
        screen.WorkingArea.X,
        screen.WorkingArea.Y
      );

      var rootContainer = _containerService.ContainerTree;
      _bus.Invoke(new AttachContainerCommand(newMonitor, rootContainer));

      ActivateWorkspaceOnMonitor(newMonitor);

      _bus.Emit(new MonitorAddedEvent(newMonitor));

      return CommandResponse.Ok;
    }

    private void ActivateWorkspaceOnMonitor(Monitor monitor)
    {
      // Get name of first workspace that is not active for that specified monitor or any.
      var inactiveWorkspaceConfig = _workspaceService.GetWorkspaceConfigToActivate(monitor);

      if (inactiveWorkspaceConfig is null)
        throw new FatalUserException("At least 1 workspace is required per monitor.");

      // Assign the workspace to the newly added monitor.
      _bus.Invoke(new ActivateWorkspaceCommand(inactiveWorkspaceConfig.Name, monitor));
    }
  }
}
