using System;
using Yugen.Infrastructure.WindowsApi;

namespace Yugen.Domain.Windows
{
  public sealed class MinimizedWindow : Window
  {
    public WindowType PreviousState;

    public MinimizedWindow(
      IntPtr handle,
      Rect floatingPlacement,
      RectDelta borderDelta,
      WindowType previousState
    ) : base(handle, floatingPlacement, borderDelta)
    {
      PreviousState = previousState;
    }
  }
}
