using Yugen.Infrastructure.Bussing;

namespace Yugen.Domain.Common.Events
{
  public record BindingModeChangedEvent(string NewBindingMode)
    : Event(DomainEvent.BindingModeChanged);
}
