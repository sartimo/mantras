using Yugen.Domain.Containers.Commands;
using Yugen.Domain.UserConfigs;
using Yugen.Infrastructure.Bussing;
using static Yugen.Infrastructure.WindowsApi.WindowsApiService;

namespace Yugen.Domain.Containers.CommandHandlers
{
  internal sealed class CenterCursorOnContainerHandler : ICommandHandler<CenterCursorOnContainerCommand>
  {
    private readonly UserConfigService _userConfigService;

    public CenterCursorOnContainerHandler(UserConfigService userConfigService)
    {
      _userConfigService = userConfigService;
    }

    public CommandResponse Handle(CenterCursorOnContainerCommand command)
    {
      var isEnabled = _userConfigService.GeneralConfig.CursorFollowsFocus;

      if (!isEnabled || command.TargetContainer.IsDetached() || command.TargetContainer.FocusIndex < 0)
        return CommandResponse.Ok;

      var targetRect = command.TargetContainer.ToRect();

      // Calculate center point of focused window.
      var centerX = targetRect.X + (targetRect.Width / 2);
      var centerY = targetRect.Y + (targetRect.Height / 2);

      SetCursorPos(centerX, centerY);

      return CommandResponse.Ok;
    }
  }
}
