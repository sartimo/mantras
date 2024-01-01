using Yugen.Domain.Common;
using Yugen.Infrastructure.Bussing;

namespace Yugen.Domain.Workspaces.Events
{
  public record WorkspaceActivatedEvent(Workspace ActivatedWorkspace)
    : Event(DomainEvent.WorkspaceActivated);
}
