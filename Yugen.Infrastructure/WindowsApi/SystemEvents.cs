using System;
using System.Reactive.Linq;
using Yugen.Infrastructure.Common.Events;

namespace Yugen.Infrastructure.WindowsApi
{
  public static class SystemEvents
  {
    public static IObservable<DisplaySettingsChangedEvent> DisplaySettingsChanged =>
      Observable.FromEventPattern<EventHandler, EventArgs>(
        handler => Microsoft.Win32.SystemEvents.DisplaySettingsChanged += handler,
        handler => Microsoft.Win32.SystemEvents.DisplaySettingsChanged -= handler
      ).Select((_) => new DisplaySettingsChangedEvent());
  }
}
