using Yugen.Domain.Containers.Commands;
using Yugen.Infrastructure.Bussing;
using Yugen.Infrastructure.Utils;

namespace Yugen.Domain.Containers.CommandHandlers
{
  internal sealed class SetFocusedDescendantHandler : ICommandHandler<SetFocusedDescendantCommand>
  {
    public CommandResponse Handle(SetFocusedDescendantCommand command)
    {
      var focusedDescendant = command.FocusedDescendant;
      var endAncestor = command.EndAncestor;

      // Traverse upwards, setting the container as the last focused until the root container
      // or `endAncestor` (if provided) is reached.
      var target = focusedDescendant;
      while (target.Parent != null && target != endAncestor)
      {
        target.Parent.ChildFocusOrder.MoveToFront(target);
        target = target.Parent;
      }

      return CommandResponse.Ok;
    }
  }
}
