using Yugen.Domain.Common.Enums;
using Yugen.Infrastructure.Bussing;

namespace Yugen.Domain.Windows.Commands
{
  public class MoveWindowCommand : Command
  {
    public Window WindowToMove { get; }
    public Direction Direction { get; }

    public MoveWindowCommand(Window windowToMove, Direction direction)
    {
      WindowToMove = windowToMove;
      Direction = direction;
    }
  }
}
