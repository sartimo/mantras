using System;
using Yugen.Infrastructure.WindowsApi;

namespace Yugen.Domain.Windows
{
  public sealed class FullscreenWindow : Window
  {
    public FullscreenWindow(
      IntPtr handle,
      Rect floatingPlacement,
      RectDelta borderDelta
    ) : base(handle, floatingPlacement, borderDelta)
    {
    }
  }
}
