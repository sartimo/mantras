using Yugen.Domain.Common;
using Yugen.Domain.Common.Enums;
using Yugen.Infrastructure.Bussing;

namespace Yugen.Domain.Containers.Events
{
  public record TilingDirectionChangedEvent(TilingDirection NewTilingDirection)
    : Event(DomainEvent.TilingDirectionChanged);
}
