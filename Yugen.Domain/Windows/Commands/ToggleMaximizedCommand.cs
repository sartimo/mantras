using Yugen.Infrastructure.Bussing;

namespace Yugen.Domain.Windows.Commands
{
  public class ToggleMaximizedCommand : Command
  {
    public Window Window { get; }

    public ToggleMaximizedCommand(Window window)
    {
      Window = window;
    }
  }
}
