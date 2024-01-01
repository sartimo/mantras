using Yugen.Domain.Common.Enums;
using Yugen.Infrastructure.Bussing;

namespace Yugen.Domain.Containers.Commands
{
  public class FocusInDirectionCommand : Command
  {
    public Direction Direction { get; }

    public FocusInDirectionCommand(Direction direction)
    {
      Direction = direction;
    }
  }
}
