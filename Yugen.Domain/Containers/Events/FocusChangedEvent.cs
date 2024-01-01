using Yugen.Domain.Common;
using Yugen.Infrastructure.Bussing;

namespace Yugen.Domain.Containers.Events
{
  public record FocusChangedEvent(Container FocusedContainer)
    : Event(DomainEvent.FocusChanged);
}
