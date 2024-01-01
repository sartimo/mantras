using Yugen.Domain.Containers.Commands;
using Yugen.Domain.Workspaces.Commands;
using Yugen.Domain.Workspaces.Events;
using Yugen.Infrastructure.Bussing;

namespace Yugen.Domain.Workspaces.CommandHandlers
{
  internal sealed class DeactivateWorkspaceHandler : ICommandHandler<DeactivateWorkspaceCommand>
  {
    public Bus _bus { get; }

    public DeactivateWorkspaceHandler(Bus bus)
    {
      _bus = bus;
    }

    public CommandResponse Handle(DeactivateWorkspaceCommand command)
    {
      var workspace = command.Workspace;

      _bus.Invoke(new DetachContainerCommand(workspace));
      _bus.Emit(new WorkspaceDeactivatedEvent(workspace.Id, workspace.Name));

      return CommandResponse.Ok;
    }
  }
}
