using Yugen.Infrastructure.Bussing;

namespace Yugen.Domain.Containers.Commands
{
  public class FlattenSplitContainerCommand : Command
  {
    public SplitContainer ContainerToFlatten { get; }

    public FlattenSplitContainerCommand(SplitContainer parent)
    {
      ContainerToFlatten = parent;
    }
  }
}
