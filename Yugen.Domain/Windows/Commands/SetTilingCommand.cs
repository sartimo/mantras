using Yugen.Infrastructure.Bussing;

namespace Yugen.Domain.Windows.Commands
{
  public class SetTilingCommand : Command
  {
    public Window Window { get; }

    public SetTilingCommand(Window window)
    {
      Window = window;
    }
  }
}
