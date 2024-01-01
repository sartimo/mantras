using Yugen.Domain.Windows.Commands;
using Yugen.Infrastructure.Bussing;
using static Yugen.Infrastructure.WindowsApi.WindowsApiService;

namespace Yugen.Domain.Windows.CommandHandlers
{
  internal sealed class ToggleMaximizedHandler : ICommandHandler<ToggleMaximizedCommand>
  {
    public CommandResponse Handle(ToggleMaximizedCommand command)
    {
      var window = command.Window;

      if (window.HasWindowStyle(WindowStyles.Maximize))
        ShowWindowAsync(window.Handle, ShowWindowFlags.Restore);
      else
        ShowWindowAsync(window.Handle, ShowWindowFlags.Maximize);

      return CommandResponse.Ok;
    }
  }
}
