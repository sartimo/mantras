using Yugen.Infrastructure.Bussing;

namespace Yugen.Domain.Containers.Commands
{
  public class DetachContainerCommand : Command
  {
    public Container ChildToRemove { get; }

    public DetachContainerCommand(Container childToRemove)
    {
      ChildToRemove = childToRemove;
    }
  }
}
