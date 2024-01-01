using System.Collections.Generic;
using Yugen.Domain.UserConfigs;
using Yugen.Infrastructure.Bussing;

namespace Yugen.Domain.Workspaces.Commands
{
  internal sealed class UpdateWorkspacesFromConfigCommand : Command
  {
    public List<WorkspaceConfig> WorkspaceConfigs { get; }

    public UpdateWorkspacesFromConfigCommand(List<WorkspaceConfig> workspaceConfigs)
    {
      WorkspaceConfigs = workspaceConfigs;
    }
  }
}
