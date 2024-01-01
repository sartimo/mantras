using Yugen.Domain.Windows.Commands;
using Yugen.Infrastructure.Bussing;

namespace Yugen.Domain.Windows.CommandHandlers
{
  internal sealed class ToggleFloatingHandler : ICommandHandler<ToggleFloatingCommand>
  {
    private readonly Bus _bus;

    public ToggleFloatingHandler(Bus bus)
    {
      _bus = bus;
    }

    public CommandResponse Handle(ToggleFloatingCommand command)
    {
      var window = command.Window;

      if (window is FloatingWindow)
        _bus.Invoke(new SetTilingCommand(window));
      else
        _bus.Invoke(new SetFloatingCommand(window));

      return CommandResponse.Ok;
    }
  }
}
