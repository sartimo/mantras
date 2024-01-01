using Yugen.Infrastructure.Bussing;
using Yugen.Infrastructure.WindowsApi;

namespace Yugen.Domain.Windows.Commands
{
  public class ResizeWindowBordersCommand : Command
  {
    public Window WindowToResize { get; }
    public RectDelta BorderDelta { get; }

    public ResizeWindowBordersCommand(Window windowToResize, RectDelta borderDelta)
    {
      WindowToResize = windowToResize;
      BorderDelta = borderDelta;
    }
  }
}
