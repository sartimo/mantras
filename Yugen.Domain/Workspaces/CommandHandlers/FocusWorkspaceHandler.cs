using System.Linq;
using Yugen.Domain.Containers;
using Yugen.Domain.Containers.Commands;
using Yugen.Domain.Containers.Events;
using Yugen.Domain.Monitors;
using Yugen.Domain.UserConfigs;
using Yugen.Domain.Workspaces.Commands;
using Yugen.Infrastructure.Bussing;
using Microsoft.Extensions.Logging;

namespace Yugen.Domain.Workspaces.CommandHandlers
{
  internal sealed class FocusWorkspaceHandler : ICommandHandler<FocusWorkspaceCommand>
  {
    private readonly Bus _bus;
    private readonly ILogger<FocusWorkspaceHandler> _logger;
    private readonly ContainerService _containerService;
    private readonly MonitorService _monitorService;
    private readonly UserConfigService _userConfigService;
    private readonly WorkspaceService _workspaceService;

    public FocusWorkspaceHandler(
      Bus bus,
      ILogger<FocusWorkspaceHandler> logger,
      ContainerService containerService,
      MonitorService monitorService,
      UserConfigService userConfigService,
      WorkspaceService workspaceService)
    {
      _bus = bus;
      _logger = logger;
      _containerService = containerService;
      _monitorService = monitorService;
      _userConfigService = userConfigService;
      _workspaceService = workspaceService;
    }

    public CommandResponse Handle(FocusWorkspaceCommand command)
    {
      var workspaceName = "";
      var focusedWorkspace = _workspaceService.GetFocusedWorkspace();
      if (focusedWorkspace.Name == command.WorkspaceName)
      {
        if (!_userConfigService.GeneralConfig.ToggleWorkspaceOnRefocus || _workspaceService.MostRecentWorkspace == null)
          return CommandResponse.Ok;
        workspaceName = _workspaceService.MostRecentWorkspace.Name;
      }
      else
      {
        workspaceName = command.WorkspaceName;
      }

      // Get workspace to focus. If it's currently inactive, then activate it.
      var workspaceToFocus = _workspaceService.GetActiveWorkspaceByName(workspaceName)
      ?? ActivateWorkspace(workspaceName);

      var displayedWorkspace = (workspaceToFocus.Parent as Monitor).DisplayedWorkspace;

      // Save currently focused workspace as recent for command "recent"
      _workspaceService.MostRecentWorkspace = focusedWorkspace;

      _logger.LogDebug("WorkspaceToFocus: {WorkspaceToFocusName}", workspaceToFocus.Name);

      // Set focus to the last focused window in workspace. If the workspace has no descendant
      // windows, then set focus to the workspace itself.
      var containerToFocus = workspaceToFocus.HasChildren()
        ? workspaceToFocus.LastFocusedDescendant
        : workspaceToFocus;

      _bus.Invoke(new SetFocusedDescendantCommand(containerToFocus));
      _containerService.HasPendingFocusSync = true;

      // Display the workspace to switch focus to.
      _containerService.ContainersToRedraw.Add(displayedWorkspace);
      _containerService.ContainersToRedraw.Add(workspaceToFocus);

      // Get empty workspace to destroy (if any are found). Cannot destroy empty workspaces if
      // they're the only workspace on the monitor or are pending focus.
      var workspaceToDestroy = _workspaceService
        .GetActiveWorkspaces()
        .FirstOrDefault(
          (workspace) =>
            !workspace.KeepAlive && !workspace.HasChildren() && !workspace.IsDisplayed
        );

      if (workspaceToDestroy != null)
        _bus.Invoke(new DeactivateWorkspaceCommand(workspaceToDestroy));

      return CommandResponse.Ok;
    }

    /// <summary>
    /// Activate a given workspace on the currently focused monitor.
    /// </summary>
    private Workspace ActivateWorkspace(string workspaceName)
    {
      // Get the monitor that the workspace should be bound to (if it exists).
      var workspaceConfig = _userConfigService.GetWorkspaceConfigByName(workspaceName);
      var boundMonitor =
        _monitorService.GetMonitorByDeviceName(workspaceConfig.BindToMonitor);

      var targetMonitor = boundMonitor ?? _monitorService.GetFocusedMonitor();
      _bus.Invoke(new ActivateWorkspaceCommand(workspaceName, targetMonitor));

      return _workspaceService.GetActiveWorkspaceByName(workspaceName);
    }
  }
}
