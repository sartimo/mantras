using Yugen.Infrastructure.Bussing;
using Yugen.Infrastructure.WindowsApi;

namespace Yugen.Domain.Containers.Commands
{
  public class FocusContainerUnderCursorCommand : Command
  {
    public Point TargetPoint { get; }
    public FocusContainerUnderCursorCommand(Point targetPoint)
    {
      TargetPoint = targetPoint;
    }
  }
}
