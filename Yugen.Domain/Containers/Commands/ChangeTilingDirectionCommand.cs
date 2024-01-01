using Yugen.Domain.Common.Enums;
using Yugen.Infrastructure.Bussing;

namespace Yugen.Domain.Containers.Commands
{
  public class ChangeTilingDirectionCommand : Command
  {
    public Container Container { get; }
    public TilingDirection TilingDirection { get; }

    public ChangeTilingDirectionCommand(
      Container container,
      TilingDirection tilingDirection)
    {
      Container = container;
      TilingDirection = tilingDirection;
    }
  }
}
