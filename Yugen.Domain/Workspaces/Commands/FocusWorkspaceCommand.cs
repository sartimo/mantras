using Yugen.Infrastructure.Bussing;

namespace Yugen.Domain.Workspaces.Commands
{
  public class FocusWorkspaceCommand : Command
  {
    public string WorkspaceName { get; }

    public FocusWorkspaceCommand(string workspaceName)
    {
      WorkspaceName = workspaceName;
    }
  }
}
