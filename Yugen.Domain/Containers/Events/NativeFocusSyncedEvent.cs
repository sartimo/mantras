using Yugen.Domain.Common;
using Yugen.Infrastructure.Bussing;

namespace Yugen.Domain.Containers.Events
{
  public record NativeFocusSyncedEvent(Container FocusedContainer)
    : Event(DomainEvent.NativeFocusSynced);
}
