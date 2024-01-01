using Yugen.Domain.Common.Enums;
using Yugen.Infrastructure.Bussing;

namespace Yugen.Domain.Workspaces.Commands
{
  internal sealed class MoveWorkspaceInDirectionCommand : Command
  {
    // TODO: Add argument for workspace to move instead of assuming the focused workspace.
    public Direction Direction { get; }

    public MoveWorkspaceInDirectionCommand(Direction direction)
    {
      Direction = direction;
    }
  }
}
