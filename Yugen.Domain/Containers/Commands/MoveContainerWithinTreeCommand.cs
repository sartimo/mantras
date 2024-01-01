using Yugen.Infrastructure.Bussing;

namespace Yugen.Domain.Containers.Commands
{
  public class MoveContainerWithinTreeCommand : Command
  {
    public Container ContainerToMove { get; }
    public Container TargetParent { get; }
    public int TargetIndex { get; }

    /// <summary>
    /// Insert child as end element if `targetIndex` is not provided.
    /// </summary>
    public MoveContainerWithinTreeCommand(
      Container containerToMove,
      Container targetParent)
    {
      ContainerToMove = containerToMove;
      TargetParent = targetParent;
      TargetIndex = targetParent.Children.Count;
    }

    public MoveContainerWithinTreeCommand(
      Container containerToMove,
      Container targetParent,
      int targetIndex)
    {
      ContainerToMove = containerToMove;
      TargetParent = targetParent;
      TargetIndex = targetIndex;
    }
  }
}
