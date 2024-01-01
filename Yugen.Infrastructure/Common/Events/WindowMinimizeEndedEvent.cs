using System;
using Yugen.Infrastructure.Bussing;

namespace Yugen.Infrastructure.Common.Events
{
  public record WindowMinimizeEndedEvent(IntPtr WindowHandle)
    : Event(InfraEvent.WindowMinimizeEnded);
}
