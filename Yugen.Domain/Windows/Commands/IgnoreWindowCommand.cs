using Yugen.Infrastructure.Bussing;

namespace Yugen.Domain.Windows.Commands
{
  public class IgnoreWindowCommand : Command
  {
    public Window WindowToIgnore { get; }

    public IgnoreWindowCommand(Window windowToIgnore)
    {
      WindowToIgnore = windowToIgnore;
    }
  }
}
