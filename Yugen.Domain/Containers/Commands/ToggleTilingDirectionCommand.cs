using Yugen.Infrastructure.Bussing;

namespace Yugen.Domain.Containers.Commands
{
  public class ToggleTilingDirectionCommand : Command
  {
    public Container Container { get; }

    public ToggleTilingDirectionCommand(Container container)
    {
      Container = container;
    }
  }
}
