using Yugen.Domain.Common.Enums;
using Yugen.Infrastructure.Bussing;

namespace Yugen.Domain.Workspaces.Commands
{
  public class FocusWorkspaceSequenceCommand : Command
  {
    public Sequence Direction { get; }

    public FocusWorkspaceSequenceCommand(Sequence direction)
    {
      Direction = direction;
    }
  }
}
