using Yugen.Domain.Containers;
using Yugen.Domain.Containers.Commands;
using Yugen.Domain.Windows.Commands;
using Yugen.Domain.Windows.Events;
using Yugen.Infrastructure.Bussing;

namespace Yugen.Domain.Windows.CommandHandlers
{
  internal sealed class UnmanageWindowHandler : ICommandHandler<UnmanageWindowCommand>
  {
    private readonly Bus _bus;
    private readonly ContainerService _containerService;
    private readonly WindowService _windowService;

    public UnmanageWindowHandler(
      Bus bus,
      ContainerService containerService,
      WindowService windowService)
    {
      _bus = bus;
      _containerService = containerService;
      _windowService = windowService;
    }

    public CommandResponse Handle(UnmanageWindowCommand command)
    {
      var window = command.Window;

      // Get container to switch focus to after the window has been removed.
      var focusedContainer = _containerService.FocusedContainer;
      var focusTarget = window == focusedContainer
        ? WindowService.GetFocusTargetAfterRemoval(window)
        : null;

      _bus.Invoke(new DetachContainerCommand(window));
      _bus.Emit(new WindowUnmanagedEvent(window.Id, window.Handle));

      if (focusTarget is null)
        return CommandResponse.Ok;

      _bus.Invoke(new SetFocusedDescendantCommand(focusTarget));
      _containerService.HasPendingFocusSync = true;
      _windowService.UnmanagedOrMinimizedStopwatch.Restart();

      return CommandResponse.Ok;
    }
  }
}
