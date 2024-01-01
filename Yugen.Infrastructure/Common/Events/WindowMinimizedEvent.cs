using System;
using Yugen.Infrastructure.Bussing;

namespace Yugen.Infrastructure.Common.Events
{
  public record WindowMinimizedEvent(IntPtr WindowHandle)
    : Event(InfraEvent.WindowMinimized);
}
