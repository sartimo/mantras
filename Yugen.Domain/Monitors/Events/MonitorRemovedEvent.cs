using System;
using Yugen.Domain.Common;
using Yugen.Infrastructure.Bussing;

namespace Yugen.Domain.Monitors.Events
{
  public record MonitorRemovedEvent(Guid RemovedId, string RemovedDeviceName)
    : Event(DomainEvent.MonitorRemoved);
}
