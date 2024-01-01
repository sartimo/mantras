using System;
using Yugen.Domain.Windows.Commands;
using Yugen.Infrastructure.Bussing;
using static Yugen.Infrastructure.WindowsApi.WindowsApiService;

namespace Yugen.Domain.Windows.CommandHandlers
{
  internal sealed class CloseWindowHandler : ICommandHandler<CloseWindowCommand>
  {
    public CommandResponse Handle(CloseWindowCommand command)
    {
      var windowToClose = command.WindowToClose;

      SendNotifyMessage(windowToClose.Handle, SendMessageType.Close, IntPtr.Zero, IntPtr.Zero);

      return CommandResponse.Ok;
    }
  }
}
