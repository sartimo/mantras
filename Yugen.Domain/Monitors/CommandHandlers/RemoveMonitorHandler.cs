using System.Linq;
using Yugen.Domain.Containers.Commands;
using Yugen.Domain.Monitors.Commands;
using Yugen.Domain.Monitors.Events;
using Yugen.Domain.Workspaces;
using Yugen.Domain.Workspaces.Events;
using Yugen.Infrastructure.Bussing;

namespace Yugen.Domain.Monitors.CommandHandlers
{
  internal sealed class RemoveMonitorHandler : ICommandHandler<RemoveMonitorCommand>
  {
    private readonly Bus _bus;
    private readonly MonitorService _monitorService;

    public RemoveMonitorHandler(Bus bus, MonitorService monitorService)
    {
      _bus = bus;
      _monitorService = monitorService;
    }

    public CommandResponse Handle(RemoveMonitorCommand command)
    {
      var monitorToRemove = command.MonitorToRemove;
      var targetMonitor = _monitorService.GetMonitors().First(
        monitor => monitor != monitorToRemove
      );

      // Keep reference to the focused monitor prior to moving workspaces around.
      var focusedMonitor = _monitorService.GetFocusedMonitor();

      // Avoid moving empty workspaces.
      var workspacesToMove = monitorToRemove.Children
        .Cast<Workspace>()
        .Where(workspace => workspace.HasChildren() || workspace.KeepAlive);

      foreach (var workspace in workspacesToMove.ToList())
      {
        // Move workspace to target monitor.
        _bus.Invoke(new MoveContainerWithinTreeCommand(workspace, targetMonitor));

        // Update workspaces displayed in bar window.
        // TODO: Consider creating separate event `WorkspaceMovedEvent`.
        _bus.Emit(new WorkspaceActivatedEvent(workspace));
      }

      _bus.Invoke(new DetachContainerCommand(monitorToRemove));
      _bus.Emit(new MonitorRemovedEvent(monitorToRemove.Id, monitorToRemove.DeviceName));

      return CommandResponse.Ok;
    }
  }
}
