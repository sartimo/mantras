using System.Linq;
using Yugen.Domain.Common.Utils;
using Yugen.Domain.Containers.Commands;
using Yugen.Domain.Monitors.Commands;
using Yugen.Domain.Windows.Commands;
using Yugen.Infrastructure.Bussing;
using Yugen.Infrastructure.Common.Events;
using Microsoft.Extensions.Logging;

namespace Yugen.Domain.Windows.EventHandlers
{
  internal sealed class WindowDestroyedHandler : IEventHandler<WindowDestroyedEvent>
  {
    private readonly Bus _bus;
    private readonly WindowService _windowService;
    private readonly ILogger<WindowDestroyedHandler> _logger;

    public WindowDestroyedHandler(
      Bus bus,
      WindowService windowService,
      ILogger<WindowDestroyedHandler> logger)
    {
      _bus = bus;
      _windowService = windowService;
      _logger = logger;
    }

    public void Handle(WindowDestroyedEvent @event)
    {
      var windowHandle = @event.WindowHandle;

      if (_windowService.AppBarHandles.Contains(windowHandle))
      {
        _windowService.AppBarHandles.Remove(windowHandle);
        _bus.Invoke(new RefreshMonitorStateCommand());
        return;
      }

      var window = _windowService.GetWindows()
        .FirstOrDefault(window => window.Handle == windowHandle);

      if (window == null)
        return;

      _logger.LogWindowEvent("Window closed", window);

      // If window is in tree, detach the removed window from its parent.
      _bus.Invoke(new UnmanageWindowCommand(window));
      _bus.Invoke(new RedrawContainersCommand());
      _bus.Invoke(new SyncNativeFocusCommand());
    }
  }
}
