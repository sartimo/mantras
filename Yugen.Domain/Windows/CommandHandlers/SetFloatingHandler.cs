using Yugen.Domain.Containers;
using Yugen.Domain.Containers.Commands;
using Yugen.Domain.Windows.Commands;
using Yugen.Domain.Workspaces;
using Yugen.Infrastructure.Bussing;

namespace Yugen.Domain.Windows.CommandHandlers
{
  internal sealed class SetFloatingHandler : ICommandHandler<SetFloatingCommand>
  {
    private readonly Bus _bus;

    public SetFloatingHandler(Bus bus)
    {
      _bus = bus;
    }

    public CommandResponse Handle(SetFloatingCommand command)
    {
      var window = command.Window;

      if (window is FloatingWindow)
        return CommandResponse.Ok;

      // Keep reference to the window's ancestor workspace prior to detaching.
      var workspace = WorkspaceService.GetWorkspaceFromChildContainer(window);

      _bus.Invoke(new MoveContainerWithinTreeCommand(window, workspace));

      // Create a floating window and place it in the center of the workspace.
      var floatingWindow = new FloatingWindow(
        window.Handle,
        window.FloatingPlacement,
        window.BorderDelta
      )
      {
        Id = window.Id
      };

      _bus.Invoke(new ReplaceContainerCommand(floatingWindow, window.Parent, window.Index));

      return CommandResponse.Ok;
    }
  }
}
