using System;
using Yugen.Domain.Containers;
using Yugen.Infrastructure;
using Yugen.Infrastructure.WindowsApi;

namespace Yugen.Domain.Windows
{
  public sealed class TilingWindow : Window, IResizable
  {
    public double SizePercentage { get; set; } = 1;

    private readonly ContainerService _containerService =
      ServiceLocator.GetRequiredService<ContainerService>();

    public override int Width => _containerService.GetWidthOfResizableContainer(this);
    public override int Height => _containerService.GetHeightOfResizableContainer(this);
    public override int X => _containerService.GetXOfResizableContainer(this);
    public override int Y => _containerService.GetYOfResizableContainer(this);

    public TilingWindow(
      IntPtr handle,
      Rect floatingPlacement,
      RectDelta borderDelta
    ) : base(handle, floatingPlacement, borderDelta)
    {
    }

    public TilingWindow(
      IntPtr handle,
      Rect floatingPlacement,
      RectDelta borderDelta,
      double sizePercentage
    ) : base(handle, floatingPlacement, borderDelta)
    {
      SizePercentage = sizePercentage;
    }
  }
}
