using Yugen.Domain.Common;
using Yugen.Infrastructure.Bussing;

namespace Yugen.Domain.Monitors.Events
{
  public record MonitorAddedEvent(Monitor AddedMonitor)
    : Event(DomainEvent.MonitorAdded);
}
