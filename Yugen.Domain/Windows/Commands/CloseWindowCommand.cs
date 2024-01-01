using Yugen.Infrastructure.Bussing;

namespace Yugen.Domain.Windows.Commands
{
  public class CloseWindowCommand : Command
  {
    public Window WindowToClose { get; }

    public CloseWindowCommand(Window windowToClose)
    {
      WindowToClose = windowToClose;
    }
  }
}
