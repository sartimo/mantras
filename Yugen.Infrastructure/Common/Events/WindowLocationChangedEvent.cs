using System;
using Yugen.Infrastructure.Bussing;

namespace Yugen.Infrastructure.Common.Events
{
  public record WindowLocationChangedEvent(IntPtr WindowHandle)
    : Event(InfraEvent.WindowLocationChanged);
}
