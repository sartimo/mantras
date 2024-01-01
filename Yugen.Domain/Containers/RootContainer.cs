using Yugen.Domain.Common;

namespace Yugen.Domain.Containers
{
  public sealed class RootContainer : Container
  {
    /// <inheritdoc />
    public override ContainerType Type { get; } = ContainerType.Root;
  }
}
