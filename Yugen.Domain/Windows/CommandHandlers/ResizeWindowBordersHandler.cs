using Yugen.Domain.Containers;
using Yugen.Domain.Containers.Commands;
using Yugen.Domain.Windows.Commands;
using Yugen.Infrastructure.Bussing;
using Yugen.Infrastructure.WindowsApi;

namespace Yugen.Domain.Windows.CommandHandlers
{
  internal sealed class ResizeWindowBordersHandler : ICommandHandler<ResizeWindowBordersCommand>
  {
    private readonly Bus _bus;
    private readonly ContainerService _containerService;

    public ResizeWindowBordersHandler(Bus bus, ContainerService containerService)
    {
      _bus = bus;
      _containerService = containerService;
    }

    public CommandResponse Handle(ResizeWindowBordersCommand command)
    {
      var borderDelta = command.BorderDelta;
      var windowToResize = command.WindowToResize;

      // Set the new border delta of the window.
      // TODO: Move default border delta into some sort of shared state.
      var defaultBorderDelta = new RectDelta(7, 0, 7, 7);
      windowToResize.BorderDelta = new RectDelta(
        defaultBorderDelta.Left + borderDelta.Left,
        defaultBorderDelta.Top + borderDelta.Top,
        defaultBorderDelta.Right + borderDelta.Right,
        defaultBorderDelta.Bottom + borderDelta.Bottom
      );

      // No need to redraw if window isn't tiling.
      if (windowToResize is not TilingWindow)
        return CommandResponse.Ok;

      _containerService.ContainersToRedraw.Add(windowToResize);

      return CommandResponse.Ok;
    }
  }
}
