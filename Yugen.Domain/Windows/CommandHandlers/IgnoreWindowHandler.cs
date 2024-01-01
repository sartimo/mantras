using Yugen.Domain.Containers;
using Yugen.Domain.Containers.Commands;
using Yugen.Domain.Windows.Commands;
using Yugen.Infrastructure.Bussing;

namespace Yugen.Domain.Windows.CommandHandlers
{
  internal sealed class IgnoreWindowHandler : ICommandHandler<IgnoreWindowCommand>
  {
    private readonly Bus _bus;
    private readonly WindowService _windowService;

    public IgnoreWindowHandler(Bus bus, WindowService windowService)
    {
      _bus = bus;
      _windowService = windowService;
    }

    public CommandResponse Handle(IgnoreWindowCommand command)
    {
      var windowToIgnore = command.WindowToIgnore;

      // Store handle to ignored window.
      _windowService.IgnoredHandles.Add(windowToIgnore.Handle);

      _bus.Invoke(new DetachContainerCommand(windowToIgnore));

      return CommandResponse.Ok;
    }
  }
}
