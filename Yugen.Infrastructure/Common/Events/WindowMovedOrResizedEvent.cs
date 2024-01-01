using System;
using Yugen.Infrastructure.Bussing;

namespace Yugen.Infrastructure.Common.Events
{
  public record WindowMovedOrResizedEvent(IntPtr WindowHandle)
    : Event(InfraEvent.WindowMovedOrResized);
}
