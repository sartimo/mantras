using System;
using Yugen.Infrastructure.Bussing;

namespace Yugen.Infrastructure.Common.Events
{
  public record WindowDestroyedEvent(IntPtr WindowHandle)
    : Event(InfraEvent.WindowDestroyed);
}
