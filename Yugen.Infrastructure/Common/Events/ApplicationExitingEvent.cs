using Yugen.Infrastructure.Bussing;

namespace Yugen.Infrastructure.Common.Events
{
  public record ApplicationExitingEvent() : Event(InfraEvent.ApplicationExiting);
}
