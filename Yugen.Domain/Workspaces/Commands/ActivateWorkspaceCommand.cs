using Yugen.Domain.Monitors;
using Yugen.Infrastructure.Bussing;

namespace Yugen.Domain.Workspaces.Commands
{
  internal sealed class ActivateWorkspaceCommand : Command
  {
    public string WorkspaceName { get; }
    public Monitor TargetMonitor { get; }

    public ActivateWorkspaceCommand(string workspaceName, Monitor targetMonitor)
    {
      WorkspaceName = workspaceName;
      TargetMonitor = targetMonitor;
    }
  }
}
