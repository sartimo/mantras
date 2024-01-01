using System.Linq;
using Yugen.Domain.Common.Commands;
using Yugen.Domain.Containers;
using Yugen.Domain.Containers.Commands;
using Yugen.Domain.Monitors;
using Yugen.Domain.Monitors.Commands;
using Yugen.Domain.UserConfigs.Commands;
using Yugen.Domain.Windows;
using Yugen.Domain.Windows.Commands;
using Yugen.Domain.Workspaces;
using Yugen.Infrastructure.Bussing;
using static Yugen.Infrastructure.WindowsApi.WindowsApiService;

namespace Yugen.Domain.Common.CommandHandlers
{
  internal sealed class PopulateInitialStateHandler : ICommandHandler<PopulateInitialStateCommand>
  {
    private readonly Bus _bus;
    private readonly ContainerService _containerService;
    private readonly MonitorService _monitorService;
    private readonly WindowService _windowService;
    private readonly WorkspaceService _workspaceService;

    public PopulateInitialStateHandler(
      Bus bus,
      ContainerService containerService,
      MonitorService monitorService,
      WindowService windowService,
      WorkspaceService workspaceService)
    {
      _bus = bus;
      _containerService = containerService;
      _monitorService = monitorService;
      _windowService = windowService;
      _workspaceService = workspaceService;
    }

    public CommandResponse Handle(PopulateInitialStateCommand command)
    {
      // Read user config file and set its values in state.
      _bus.Invoke(new EvaluateUserConfigCommand());

      var focusedHandle = GetForegroundWindow();
      PopulateContainerTree();

      // Register appbar windows.
      foreach (var windowHandle in WindowService.GetAllWindowHandles())
        if (_windowService.IsHandleAppBar(windowHandle))
          _windowService.AppBarHandles.Add(windowHandle);

      // Get the originally focused window when the WM is started.
      var focusedWindow =
        _windowService.GetWindows().FirstOrDefault(window => window.Handle == focusedHandle);

      // `GetForegroundWindow` might return a handle that is not in the tree. In that case, set
      // focus to an arbitrary window. If there are no manageable windows in the tree, set focus to
      // an arbitrary workspace.
      var containerToFocus =
        focusedWindow ??
        _windowService.GetWindows().FirstOrDefault() ??
        _workspaceService.GetActiveWorkspaces().FirstOrDefault() as Container;

      _bus.Invoke(new SetFocusedDescendantCommand(containerToFocus));
      _containerService.HasPendingFocusSync = true;

      return CommandResponse.Ok;
    }

    private void PopulateContainerTree()
    {
      // Create a Monitor and consequently a Workspace for each detected Screen. `AllScreens` is an
      // abstraction over `EnumDisplayMonitors` native method.
      foreach (var screen in System.Windows.Forms.Screen.AllScreens)
        _bus.Invoke(new AddMonitorCommand(screen));

      // Add initial windows to the tree.
      // TODO: Copy all the below over to populate with cache method, but filter out window handles
      // that have already been added to state.
      foreach (var windowHandle in WindowService.GetAllWindowHandles())
      {
        if (!WindowService.IsHandleManageable(windowHandle))
          continue;

        // Get workspace that encompasses most of the window.
        var targetMonitor = _monitorService.GetMonitorFromHandleLocation(windowHandle);
        var targetWorkspace = targetMonitor.DisplayedWorkspace;

        _bus.Invoke(new ManageWindowCommand(windowHandle, targetWorkspace));
      }
    }
  }
}
