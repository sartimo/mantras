using System;
using Yugen.Infrastructure.Bussing;

namespace Yugen.Infrastructure.Common.Events
{
  public record WindowFocusedEvent(IntPtr WindowHandle) : Event(InfraEvent.WindowFocused);
}
