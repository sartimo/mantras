using Yugen.Domain.Common.CommandHandlers;
using Yugen.Domain.Common.Commands;
using Yugen.Domain.Containers;
using Yugen.Domain.Containers.CommandHandlers;
using Yugen.Domain.Containers.Commands;
using Yugen.Domain.Monitors;
using Yugen.Domain.Monitors.CommandHandlers;
using Yugen.Domain.Monitors.Commands;
using Yugen.Domain.Monitors.EventHandlers;
using Yugen.Domain.UserConfigs;
using Yugen.Domain.UserConfigs.CommandHandlers;
using Yugen.Domain.UserConfigs.Commands;
using Yugen.Domain.Windows;
using Yugen.Domain.Windows.CommandHandlers;
using Yugen.Domain.Windows.Commands;
using Yugen.Domain.Windows.EventHandlers;
using Yugen.Domain.Workspaces;
using Yugen.Domain.Workspaces.CommandHandlers;
using Yugen.Domain.Workspaces.Commands;
using Yugen.Infrastructure.Bussing;
using Yugen.Infrastructure.Common.Events;
using Microsoft.Extensions.DependencyInjection;

namespace Yugen.Domain
{
  public static class DependencyInjection
  {
    public static IServiceCollection AddDomainServices(this IServiceCollection services)
    {
      services.AddSingleton<ContainerService>();
      services.AddSingleton<MonitorService>();
      services.AddSingleton<CommandParsingService>();
      services.AddSingleton<UserConfigService>();
      services.AddSingleton<WindowService>();
      services.AddSingleton<WorkspaceService>();

      services.AddSingleton<ICommandHandler<PopulateInitialStateCommand>, PopulateInitialStateHandler>();
      services.AddSingleton<ICommandHandler<ExecProcessCommand>, ExecProcessHandler>();
      services.AddSingleton<ICommandHandler<SetBindingModeCommand>, SetBindingModeHandler>();
      services.AddSingleton<ICommandHandler<AttachContainerCommand>, AttachContainerHandler>();
      services.AddSingleton<ICommandHandler<CenterCursorOnContainerCommand>, CenterCursorOnContainerHandler>();
      services.AddSingleton<ICommandHandler<SetActiveWindowBorderCommand>, SetActiveWindowBorderHandler>();
      services.AddSingleton<ICommandHandler<ChangeTilingDirectionCommand>, ChangeTilingDirectionHandler>();
      services.AddSingleton<ICommandHandler<ToggleTilingDirectionCommand>, ToggleTilingDirectionHandler>();
      services.AddSingleton<ICommandHandler<DetachContainerCommand>, DetachContainerHandler>();
      services.AddSingleton<ICommandHandler<FlattenSplitContainerCommand>, FlattenSplitContainerHandler>();
      services.AddSingleton<ICommandHandler<FocusInDirectionCommand>, FocusInDirectionHandler>();
      services.AddSingleton<ICommandHandler<MoveContainerWithinTreeCommand>, MoveContainerWithinTreeHandler>();
      services.AddSingleton<ICommandHandler<RedrawContainersCommand>, RedrawContainersHandler>();
      services.AddSingleton<ICommandHandler<ReplaceContainerCommand>, ReplaceContainerHandler>();
      services.AddSingleton<ICommandHandler<ResizeContainerCommand>, ResizeContainerHandler>();
      services.AddSingleton<ICommandHandler<SetFocusedDescendantCommand>, SetFocusedDescendantHandler>();
      services.AddSingleton<ICommandHandler<SyncNativeFocusCommand>, SyncNativeFocusHandler>();
      services.AddSingleton<ICommandHandler<ToggleFocusModeCommand>, ToggleFocusModeHandler>();
      services.AddSingleton<ICommandHandler<AddMonitorCommand>, AddMonitorHandler>();
      services.AddSingleton<ICommandHandler<RefreshMonitorStateCommand>, RefreshMonitorStateHandler>();
      services.AddSingleton<ICommandHandler<RemoveMonitorCommand>, RemoveMonitorHandler>();
      services.AddSingleton<ICommandHandler<EvaluateUserConfigCommand>, EvaluateUserConfigHandler>();
      services.AddSingleton<ICommandHandler<RegisterKeybindingsCommand>, RegisterKeybindingsHandler>();
      services.AddSingleton<ICommandHandler<ReloadUserConfigCommand>, ReloadUserConfigHandler>();
      services.AddSingleton<ICommandHandler<RunWithSubjectContainerCommand>, RunWithSubjectContainerHandler>();
      services.AddSingleton<ICommandHandler<CloseWindowCommand>, CloseWindowHandler>();
      services.AddSingleton<ICommandHandler<IgnoreWindowCommand>, IgnoreWindowHandler>();
      services.AddSingleton<ICommandHandler<ManageWindowCommand>, ManageWindowHandler>();
      services.AddSingleton<ICommandHandler<MoveWindowCommand>, MoveWindowHandler>();
      services.AddSingleton<ICommandHandler<ResizeWindowCommand>, ResizeWindowHandler>();
      services.AddSingleton<ICommandHandler<ResizeWindowBordersCommand>, ResizeWindowBordersHandler>();
      services.AddSingleton<ICommandHandler<SetFloatingCommand>, SetFloatingHandler>();
      services.AddSingleton<ICommandHandler<SetMaximizedCommand>, SetMaximizedHandler>();
      services.AddSingleton<ICommandHandler<SetMinimizedCommand>, SetMinimizedHandler>();
      services.AddSingleton<ICommandHandler<SetTilingCommand>, SetTilingHandler>();
      services.AddSingleton<ICommandHandler<SetWindowSizeCommand>, SetWindowSizeHandler>();
      services.AddSingleton<ICommandHandler<ToggleFloatingCommand>, ToggleFloatingHandler>();
      services.AddSingleton<ICommandHandler<ToggleMaximizedCommand>, ToggleMaximizedHandler>();
      services.AddSingleton<ICommandHandler<UnmanageWindowCommand>, UnmanageWindowHandler>();
      services.AddSingleton<ICommandHandler<ActivateWorkspaceCommand>, ActivateWorkspaceHandler>();
      services.AddSingleton<ICommandHandler<DeactivateWorkspaceCommand>, DeactivateWorkspaceHandler>();
      services.AddSingleton<ICommandHandler<FocusWorkspaceCommand>, FocusWorkspaceHandler>();
      services.AddSingleton<ICommandHandler<FocusWorkspaceRecentCommand>, FocusWorkspaceRecentHandler>();
      services.AddSingleton<ICommandHandler<FocusWorkspaceSequenceCommand>, FocusWorkspaceSequenceHandler>();
      services.AddSingleton<ICommandHandler<FocusContainerUnderCursorCommand>, FocusContainerUnderCursorHandler>();
      services.AddSingleton<ICommandHandler<MoveWindowToWorkspaceCommand>, MoveWindowToWorkspaceHandler>();
      services.AddSingleton<ICommandHandler<UpdateWorkspacesFromConfigCommand>, UpdateWorkspacesFromConfigHandler>();
      services.AddSingleton<ICommandHandler<MoveWorkspaceInDirectionCommand>, MoveWorkspaceInDirectionHandler>();

      services.AddSingleton<IEventHandler<DisplaySettingsChangedEvent>, DisplaySettingsChangedHandler>();
      services.AddSingleton<IEventHandler<WindowDestroyedEvent>, WindowDestroyedHandler>();
      services.AddSingleton<IEventHandler<WindowFocusedEvent>, WindowFocusedHandler>();
      services.AddSingleton<IEventHandler<WindowHiddenEvent>, WindowHiddenHandler>();
      services.AddSingleton<IEventHandler<WindowLocationChangedEvent>, WindowLocationChangedHandler>();
      services.AddSingleton<IEventHandler<WindowMinimizedEvent>, WindowMinimizedHandler>();
      services.AddSingleton<IEventHandler<WindowMinimizeEndedEvent>, WindowMinimizeEndedHandler>();
      services.AddSingleton<IEventHandler<WindowMovedOrResizedEvent>, WindowMovedOrResizedHandler>();
      services.AddSingleton<IEventHandler<WindowShownEvent>, WindowShownHandler>();

      return services;
    }
  }
}
