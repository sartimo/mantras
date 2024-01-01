using Yugen.Infrastructure.Bussing;
using Yugen.Infrastructure.Common.Commands;

namespace Yugen.Infrastructure.Common.CommandHandlers
{
  internal class NoopHandler : ICommandHandler<NoopCommand>
  {
    public CommandResponse Handle(NoopCommand command)
    {
      return CommandResponse.Ok;
    }
  }
}
