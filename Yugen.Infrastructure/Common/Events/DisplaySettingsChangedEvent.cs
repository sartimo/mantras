using Yugen.Infrastructure.Bussing;

namespace Yugen.Infrastructure.Common.Events
{
  public record DisplaySettingsChangedEvent() : Event(InfraEvent.DisplaySettingsChanged);
}
