using Yugen.Domain.Common.Enums;
using Yugen.Infrastructure.Bussing;

namespace Yugen.Domain.Windows.Commands
{
  public class SetWindowSizeCommand : Command
  {
    public Window WindowToResize { get; }
    public ResizeDimension DimensionToResize { get; }
    public string ResizeAmount { get; }

    public SetWindowSizeCommand(
      Window windowToResize,
      ResizeDimension dimensionToResize,
      string resizeAmount)
    {
      WindowToResize = windowToResize;
      DimensionToResize = dimensionToResize;
      ResizeAmount = resizeAmount;
    }
  }
}
