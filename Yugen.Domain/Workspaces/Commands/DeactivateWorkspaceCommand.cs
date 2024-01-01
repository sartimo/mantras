using Yugen.Infrastructure.Bussing;

namespace Yugen.Domain.Workspaces.Commands
{
  internal sealed class DeactivateWorkspaceCommand : Command
  {
    public Workspace Workspace { get; }

    public DeactivateWorkspaceCommand(Workspace workspace)
    {
      Workspace = workspace;
    }
  }
}
