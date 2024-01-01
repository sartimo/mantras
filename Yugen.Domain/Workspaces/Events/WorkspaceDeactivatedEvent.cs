using System;
using Yugen.Domain.Common;
using Yugen.Infrastructure.Bussing;

namespace Yugen.Domain.Workspaces.Events
{
  public record WorkspaceDeactivatedEvent(Guid DeactivatedId, string DeactivatedName)
    : Event(DomainEvent.WorkspaceDeactivated);
}
