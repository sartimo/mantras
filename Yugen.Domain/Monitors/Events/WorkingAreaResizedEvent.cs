using Yugen.Domain.Common;
using Yugen.Infrastructure.Bussing;

namespace Yugen.Domain.Monitors.Events
{
  public record WorkingAreaResizedEvent(Monitor AffectedMonitor)
    : Event(DomainEvent.WorkingAreaResized);
}
