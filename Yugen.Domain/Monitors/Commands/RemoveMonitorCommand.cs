using Yugen.Infrastructure.Bussing;

namespace Yugen.Domain.Monitors.Commands
{
  public class RemoveMonitorCommand : Command
  {
    public Monitor MonitorToRemove { get; set; }

    public RemoveMonitorCommand(Monitor monitorToRemove)
    {
      MonitorToRemove = monitorToRemove;
    }
  }
}
