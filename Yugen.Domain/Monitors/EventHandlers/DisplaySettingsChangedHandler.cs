using Yugen.Domain.Monitors.Commands;
using Yugen.Infrastructure.Bussing;
using Yugen.Infrastructure.Common.Events;

namespace Yugen.Domain.Monitors.EventHandlers
{
  internal sealed class DisplaySettingsChangedHandler : IEventHandler<DisplaySettingsChangedEvent>
  {
    private readonly Bus _bus;

    public DisplaySettingsChangedHandler(Bus bus)
    {
      _bus = bus;
    }

    public void Handle(DisplaySettingsChangedEvent @event)
    {
      _bus.Invoke(new RefreshMonitorStateCommand());
    }
  }
}
