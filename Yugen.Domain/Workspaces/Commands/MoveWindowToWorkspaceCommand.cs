using Yugen.Domain.Windows;
using Yugen.Infrastructure.Bussing;

namespace Yugen.Domain.Workspaces.Commands
{
  internal sealed class MoveWindowToWorkspaceCommand : Command
  {
    public Window WindowToMove { get; }
    public string WorkspaceName { get; }

    public MoveWindowToWorkspaceCommand(Window windowToMove, string workspaceName)
    {
      WindowToMove = windowToMove;
      WorkspaceName = workspaceName;
    }
  }
}
