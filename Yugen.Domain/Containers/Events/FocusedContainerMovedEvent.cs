using Yugen.Domain.Common;
using Yugen.Infrastructure.Bussing;

namespace Yugen.Domain.Containers.Events
{
  public record FocusedContainerMovedEvent(Container FocusedContainer)
    : Event(DomainEvent.FocusedContainerMoved);
}
