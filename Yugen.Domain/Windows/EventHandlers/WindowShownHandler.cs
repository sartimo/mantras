using System.Linq;
using Yugen.Domain.Common.Enums;
using Yugen.Domain.Common.Utils;
using Yugen.Domain.Containers.Commands;
using Yugen.Domain.Monitors.Commands;
using Yugen.Domain.Windows.Commands;
using Yugen.Infrastructure.Bussing;
using Yugen.Infrastructure.Common.Events;
using Microsoft.Extensions.Logging;

namespace Yugen.Domain.Windows.EventHandlers
{
  internal sealed class WindowShownHandler : IEventHandler<WindowShownEvent>
  {
    private readonly Bus _bus;
    private readonly ILogger<WindowShownHandler> _logger;
    private readonly WindowService _windowService;

    public WindowShownHandler(
      Bus bus,
      ILogger<WindowShownHandler> logger,
      WindowService windowService)
    {
      _bus = bus;
      _logger = logger;
      _windowService = windowService;
    }

    public void Handle(WindowShownEvent @event)
    {
      var windowHandle = @event.WindowHandle;

      if (_windowService.IsHandleAppBar(windowHandle))
      {
        _windowService.AppBarHandles.Add(windowHandle);
        _bus.Invoke(new RefreshMonitorStateCommand());
        return;
      }

      var window = _windowService.GetWindows()
        .FirstOrDefault(window => window.Handle == windowHandle);

      // Manage the window if it's manageable.
      if (window is null && WindowService.IsHandleManageable(windowHandle))
      {
        _bus.Invoke(new ManageWindowCommand(windowHandle));
        _bus.Invoke(new RedrawContainersCommand());
        _bus.Invoke(new SyncNativeFocusCommand());
        return;
      }

      if (window is not null)
        _logger.LogWindowEvent("Showing window", window);

      // Update display state if window is already managed.
      if (window?.DisplayState == DisplayState.Showing)
        window.DisplayState = DisplayState.Shown;
    }
  }
}
