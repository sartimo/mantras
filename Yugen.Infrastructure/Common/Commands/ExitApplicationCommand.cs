using Yugen.Infrastructure.Bussing;

namespace Yugen.Infrastructure.Common.Commands
{
  public class ExitApplicationCommand : Command
  {
    public bool WithErrorCode { get; }

    public ExitApplicationCommand(bool withErrorCode)
    {
      WithErrorCode = withErrorCode;
    }
  }
}
