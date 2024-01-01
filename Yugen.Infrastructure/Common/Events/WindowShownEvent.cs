using System;
using Yugen.Infrastructure.Bussing;

namespace Yugen.Infrastructure.Common.Events
{
  public record WindowShownEvent(IntPtr WindowHandle) : Event(InfraEvent.WindowShown);
}
