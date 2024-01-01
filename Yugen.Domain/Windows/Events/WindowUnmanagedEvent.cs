using System;
using Yugen.Domain.Common;
using Yugen.Infrastructure.Bussing;

namespace Yugen.Domain.Windows.Events
{
  public record WindowUnmanagedEvent(Guid UnmanagedId, IntPtr UnmanagedHandle)
    : Event(DomainEvent.WindowUnmanaged);
}
