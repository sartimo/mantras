using Yugen.Domain.Monitors;
using Yugen.Domain.Workspaces.Commands;
using Yugen.Infrastructure.Bussing;
using Yugen.Infrastructure.Exceptions;

namespace Yugen.Domain.Workspaces.CommandHandlers
{
  internal sealed class UpdateWorkspacesFromConfigHandler :
    ICommandHandler<UpdateWorkspacesFromConfigCommand>
  {
    private readonly WorkspaceService _workspaceService;

    public UpdateWorkspacesFromConfigHandler(WorkspaceService workspaceService)
    {
      _workspaceService = workspaceService;
    }

    public CommandResponse Handle(UpdateWorkspacesFromConfigCommand command)
    {
      var workspaceConfigs = command.WorkspaceConfigs;

      foreach (var workspace in _workspaceService.GetActiveWorkspaces())
      {
        var workspaceConfig = workspaceConfigs.Find(config => config.Name == workspace.Name);

        if (workspaceConfig is null)
        {
          var monitor = workspace.Parent as Monitor;
          var inactiveWorkspaceConfig = _workspaceService.GetWorkspaceConfigToActivate(monitor);

          if (inactiveWorkspaceConfig is null)
            throw new FatalUserException("At least 1 workspace is required per monitor.");

          workspace.Name = inactiveWorkspaceConfig.Name;
        }

        // TODO: Update `DisplayName` and `KeepAlive` once they are changed to properties.
      }

      return CommandResponse.Ok;
    }
  }
}
