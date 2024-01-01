using Yugen.Domain.Windows.Commands;
using Yugen.Infrastructure.Bussing;
using static Yugen.Infrastructure.WindowsApi.WindowsApiService;

namespace Yugen.Domain.Windows.CommandHandlers
{
  internal sealed class SetMinimizedHandler : ICommandHandler<SetMinimizedCommand>
  {
    public CommandResponse Handle(SetMinimizedCommand command)
    {
      var window = command.Window;

      ShowWindowAsync(window.Handle, ShowWindowFlags.Minimize);

      return CommandResponse.Ok;
    }
  }
}
