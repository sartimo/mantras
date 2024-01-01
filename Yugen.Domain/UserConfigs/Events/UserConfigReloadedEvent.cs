using Yugen.Domain.Common;
using Yugen.Infrastructure.Bussing;

namespace Yugen.Domain.UserConfigs.Events
{
  public record UserConfigReloadedEvent() : Event(DomainEvent.UserConfigReloaded);
}
