using System;
using System.Diagnostics;
using Yugen.Domain.Containers.Commands;
using Yugen.Domain.UserConfigs;
using Yugen.Domain.Windows;
using Yugen.Infrastructure.Bussing;
using static Yugen.Infrastructure.WindowsApi.WindowsApiService;

namespace Yugen.Domain.Containers.CommandHandlers
{
  internal sealed class SetActiveWindowBorderHandler : ICommandHandler<SetActiveWindowBorderCommand>
  {
    private readonly UserConfigService _userConfigService;
    private static Window _lastFocused;

    private uint rgbToUint(string rgb)
    {
      var c = rgb.ToCharArray();
      var bgr = string.Concat(c[5], c[6], c[3], c[4], c[1], c[2]);
      return Convert.ToUInt32(bgr, 16);
    }

    public SetActiveWindowBorderHandler(UserConfigService userConfigService)
    {
      _userConfigService = userConfigService;
    }

    public CommandResponse Handle(SetActiveWindowBorderCommand command)
    {
      uint BorderColorAttribute = 34;
      if (_lastFocused is not null)
      {
        // Clear old window border
        var inactiveColor = _userConfigService.FocusBorderConfig.Inactive.Enabled
          ? rgbToUint(_userConfigService.FocusBorderConfig.Inactive.Color)
          : 0xFFFFFFFF;
        _ = DwmSetWindowAttribute(_lastFocused.Handle, BorderColorAttribute, ref inactiveColor, 4);
      }

      var newWindowFocused = command.TargetWindow;
      if (newWindowFocused is null)
        return CommandResponse.Ok;

      _lastFocused = command.TargetWindow;
      // Set new window border
      var activeColor = _userConfigService.FocusBorderConfig.Active.Enabled
        ? rgbToUint(_userConfigService.FocusBorderConfig.Active.Color)
        : 0xFFFFFFFF;
      _ = DwmSetWindowAttribute(_lastFocused.Handle, BorderColorAttribute, ref activeColor, 4);
      return CommandResponse.Ok;
    }
  }
}
