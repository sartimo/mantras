using Yugen.Infrastructure.Bussing;

namespace Yugen.Domain.Windows.Commands
{
  public class SetFloatingCommand : Command
  {
    public Window Window { get; }

    public SetFloatingCommand(Window window)
    {
      Window = window;
    }
  }
}
