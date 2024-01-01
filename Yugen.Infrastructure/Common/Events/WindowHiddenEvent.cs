using System;
using Yugen.Infrastructure.Bussing;

namespace Yugen.Infrastructure.Common.Events
{
  public record WindowHiddenEvent(IntPtr WindowHandle) : Event(InfraEvent.WindowHidden);
}
