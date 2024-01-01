using System.Windows.Forms;
using Yugen.Infrastructure.Bussing;

namespace Yugen.Domain.Monitors.Commands
{
  public class AddMonitorCommand : Command
  {
    public Screen Screen { get; set; }

    public AddMonitorCommand(Screen screen)
    {
      Screen = screen;
    }
  }
}
