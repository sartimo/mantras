using static Yugen.Infrastructure.WindowsApi.WindowsApiService;

namespace Yugen.Infrastructure.WindowsApi
{
  public static class SystemSettings
  {
    /// <summary>
    /// Modify global setting for whether window transition animations are enabled.
    /// </summary>
    public static void SetWindowAnimationsEnabled(bool enabled)
    {
      var animationInfo = AnimationInfo.Create(enabled);

      SystemParametersInfo(
        SystemParametersInfoFlags.SetAnimation,
        animationInfo.CallbackSize,
        ref animationInfo,
        0
      );
    }
  }
}
