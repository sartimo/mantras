using Yugen.Infrastructure.Bussing;

namespace Yugen.Domain.Windows.Commands
{
  public class SetMinimizedCommand : Command
  {
    public Window Window { get; }

    public SetMinimizedCommand(Window window)
    {
      Window = window;
    }
  }
}
