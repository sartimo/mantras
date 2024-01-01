using System;
using System.Linq;
using Yugen.Domain.Containers.Commands;
using Yugen.Infrastructure.Bussing;
using Yugen.Infrastructure.Utils;

namespace Yugen.Domain.Containers.CommandHandlers
{
  internal sealed class ReplaceContainerHandler : ICommandHandler<ReplaceContainerCommand>
  {
    private readonly ContainerService _containerService;

    public ReplaceContainerHandler(ContainerService containerService)
    {
      _containerService = containerService;
    }

    public CommandResponse Handle(ReplaceContainerCommand command)
    {
      var replacementContainer = command.ReplacementContainer;
      var targetParent = command.TargetParent;
      var targetIndex = command.TargetIndex;

      if (!replacementContainer.IsDetached())
        throw new Exception(
          "Cannot use an already attached container as replacement container. This is a bug."
        );

      var containerToReplace = targetParent.Children[targetIndex];

      if (containerToReplace is IResizable && replacementContainer is IResizable)
        (replacementContainer as IResizable).SizePercentage =
          (containerToReplace as IResizable).SizePercentage;

      // Adjust `SizePercentage` of siblings.
      if (containerToReplace is IResizable && replacementContainer is not IResizable)
      {
        // Get the freed up space after container is detached.
        var availableSizePercentage = (containerToReplace as IResizable).SizePercentage;

        var resizableSiblings = containerToReplace.Siblings
          .Where(container => container is IResizable);

        var sizePercentageIncrement = availableSizePercentage / resizableSiblings.Count();

        // Adjust `SizePercentage` of the siblings of the removed container.
        foreach (var sibling in resizableSiblings)
          (sibling as IResizable).SizePercentage += sizePercentageIncrement;
      }

      // Replace the container at the given index.
      targetParent.Children.Replace(containerToReplace, replacementContainer);
      replacementContainer.Parent = targetParent;
      targetParent.ChildFocusOrder.Replace(containerToReplace, replacementContainer);

      _containerService.ContainersToRedraw.Add(targetParent);

      return CommandResponse.Ok;
    }
  }
}
