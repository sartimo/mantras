using System;
using System.Linq;
using Yugen.Domain.Containers.Commands;
using Yugen.Infrastructure.Bussing;

namespace Yugen.Domain.Containers.CommandHandlers
{
  internal sealed class AttachContainerHandler : ICommandHandler<AttachContainerCommand>
  {
    private readonly Bus _bus;
    private readonly ContainerService _containerService;

    public AttachContainerHandler(Bus bus, ContainerService containerService)
    {
      _bus = bus;
      _containerService = containerService;
    }

    public CommandResponse Handle(AttachContainerCommand command)
    {
      var childToAdd = command.ChildToAdd;
      var targetParent = command.TargetParent;
      var targetIndex = command.TargetIndex;

      if (!childToAdd.IsDetached())
        throw new Exception("Cannot attach an already attached container. This is a bug.");

      targetParent.InsertChild(targetIndex, childToAdd);

      if (childToAdd is IResizable)
        ResizeAttachedContainer(childToAdd);

      _containerService.ContainersToRedraw.Add(targetParent);

      return CommandResponse.Ok;
    }

    public void ResizeAttachedContainer(Container attachedContainer)
    {
      var resizableSiblings = attachedContainer.SiblingsOfType<IResizable>();

      if (!resizableSiblings.Any())
      {
        (attachedContainer as IResizable).SizePercentage = 1;
        return;
      }

      var defaultPercent = 1.0 / (resizableSiblings.Count() + 1);

      // Set initial size percentage to 0, and then size up the container to `defaultPercent`.
      (attachedContainer as IResizable).SizePercentage = 0;
      _bus.Invoke(new ResizeContainerCommand(attachedContainer, defaultPercent));
    }
  }
}
