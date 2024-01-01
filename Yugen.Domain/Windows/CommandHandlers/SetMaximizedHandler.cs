using Yugen.Domain.Windows.Commands;
using Yugen.Infrastructure.Bussing;
using static Yugen.Infrastructure.WindowsApi.WindowsApiService;

namespace Yugen.Domain.Windows.CommandHandlers
{
  internal sealed class SetMaximizedHandler : ICommandHandler<SetMaximizedCommand>
  {
    public CommandResponse Handle(SetMaximizedCommand command)
    {
      var window = command.Window;

      ShowWindowAsync(window.Handle, ShowWindowFlags.Maximize);

      return CommandResponse.Ok;
    }
  }
}
