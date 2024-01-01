using Yugen.Infrastructure.Bussing;

namespace Yugen.Domain.Windows.Commands
{
  public class UnmanageWindowCommand : Command
  {
    public Window Window { get; }

    public UnmanageWindowCommand(Window window)
    {
      Window = window;
    }
  }
}
