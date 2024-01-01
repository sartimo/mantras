using Yugen.Domain.Common;
using Yugen.Infrastructure.Bussing;

namespace Yugen.Domain.Windows.Events
{
  public record WindowManagedEvent(Window ManagedWindow)
    : Event(DomainEvent.WindowManaged);
}
