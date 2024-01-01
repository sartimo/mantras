using System.Reactive.Linq;
using Yugen.Domain.Common.Commands;
using Yugen.Domain.Containers.Commands;
using Yugen.Domain.Containers.Events;
using Yugen.Domain.UserConfigs;
using Yugen.Domain.UserConfigs.Commands;
using Yugen.Domain.Windows;
using Yugen.Domain.Windows.Commands;
using Yugen.Infrastructure.Bussing;
using Yugen.Infrastructure.Common;
using Yugen.Infrastructure.Common.Commands;
using Yugen.Infrastructure.Common.Events;
using Yugen.Infrastructure.WindowsApi;
using static Yugen.Infrastructure.WindowsApi.WindowsApiService;

namespace Yugen.App.WindowManager
{
  public sealed class WmStartup
  {
    private readonly Bus _bus;
    private readonly KeybindingService _keybindingService;
    private readonly WindowService _windowService;
    private readonly WindowEventService _windowEventService;
    private readonly UserConfigService _userConfigService;

    private SystemTrayIcon? _systemTrayIcon { get; set; }

    public WmStartup(
      Bus bus,
      KeybindingService keybindingService,
      WindowService windowService,
      WindowEventService windowEventService,
      UserConfigService userConfigService)
    {
      _bus = bus;
      _keybindingService = keybindingService;
      _windowService = windowService;
      _windowEventService = windowEventService;
      _userConfigService = userConfigService;
    }

    public ExitCode Run()
    {
      try
      {
        // Set the process-default DPI awareness.
        _ = SetProcessDpiAwarenessContext(DpiAwarenessContext.PerMonitorAwareV2);

        _bus.Events.OfType<ApplicationExitingEvent>()
          .Subscribe(_ => OnApplicationExit());

        // Populate initial monitors, windows, workspaces and user config.
        _bus.Invoke(new PopulateInitialStateCommand());
        _bus.Invoke(new RedrawContainersCommand());
        _bus.Invoke(new SyncNativeFocusCommand());

        // Listen on registered keybindings.
        _keybindingService.Start();

        // Listen for window events (eg. close, focus).
        _windowEventService.Start();

        // Listen for changes to display settings.
        // TODO: Unsubscribe on application exit.
        SystemEvents.DisplaySettingsChanged.Subscribe((@event) => _bus.EmitAsync(@event));

        var systemTrayIconConfig = new SystemTrayIconConfig
        {
          HoverText = "Yugen",
          IconResourceName = "Yugen.App.Resources.icon.ico",
          Actions = new Dictionary<string, Action>
          {
            { "Reload config", () => _bus.Invoke(new ReloadUserConfigCommand()) },
            { "Exit", () => _bus.Invoke(new ExitApplicationCommand(false)) },
          }
        };

        // Add application to system tray.
        _systemTrayIcon = new SystemTrayIcon(systemTrayIconConfig);
        _systemTrayIcon.Show();

        var windowAnimations = _userConfigService.GeneralConfig.WindowAnimations;

        // Enable/disable window transition animations.
        if (windowAnimations is not WindowAnimations.Unchanged)
        {
          SystemSettings.SetWindowAnimationsEnabled(
            windowAnimations is WindowAnimations.True
          );
        }

        if (_userConfigService.FocusBorderConfig.Active.Enabled ||
            _userConfigService.FocusBorderConfig.Inactive.Enabled)
        {
          _bus.Events.OfType<FocusChangedEvent>().Subscribe((@event) =>
            _bus.InvokeAsync(
              new SetActiveWindowBorderCommand(@event.FocusedContainer as Window)
            )
          );
        }

        // Hook mouse event for focus follows cursor.
        if (_userConfigService.GeneralConfig.FocusFollowsCursor)
          MouseEvents.MouseMoves.Sample(TimeSpan.FromMilliseconds(50)).Subscribe((@event) =>
          {
            if (!@event.IsLMouseDown && !@event.IsRMouseDown)
              _bus.InvokeAsync(new FocusContainerUnderCursorCommand(@event.Point));
          });

        // Setup cursor follows focus
        if (_userConfigService.GeneralConfig.CursorFollowsFocus)
        {
          var focusedContainerMoved = _bus.Events
            .OfType<FocusedContainerMovedEvent>()
            .Select(@event => @event.FocusedContainer);

          var nativeFocusSynced = _bus.Events
            .OfType<NativeFocusSyncedEvent>()
            .Select((@event) => @event.FocusedContainer);

          focusedContainerMoved.Merge(nativeFocusSynced)
            .Where(container => container is Window)
            .Subscribe((window) => _bus.InvokeAsync(new CenterCursorOnContainerCommand(window)));
        }

        System.Windows.Forms.Application.Run();
        return ExitCode.Success;
      }
      catch (Exception exception)
      {
        _bus.Invoke(new HandleFatalExceptionCommand(exception));
        return ExitCode.Error;
      }
    }

    private void OnApplicationExit()
    {
      // Show all windows regardless of whether their workspace is displayed.
      foreach (var window in _windowService.GetWindows())
        ShowWindowAsync(window.Handle, ShowWindowFlags.ShowNoActivate);

      // Clear border on the active window.
      _bus.Invoke(new SetActiveWindowBorderCommand(null));

      // Destroy the system tray icon.
      _systemTrayIcon?.Remove();

      System.Windows.Forms.Application.Exit();
    }
  }
}
