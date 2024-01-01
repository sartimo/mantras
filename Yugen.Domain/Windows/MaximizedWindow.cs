using System;
using Yugen.Infrastructure.WindowsApi;

namespace Yugen.Domain.Windows
{
  public sealed class MaximizedWindow : Window
  {
    public WindowType PreviousState;

    public MaximizedWindow(
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
