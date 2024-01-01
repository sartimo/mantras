using System;
using Yugen.Infrastructure.Bussing;

namespace Yugen.Infrastructure.Common.Events
{
  public record WindowTitleChangedEvent(IntPtr WindowHandle)
    : Event(InfraEvent.WindowTitleChanged);
}
