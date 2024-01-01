using Yugen.Domain.Windows;
using Microsoft.Extensions.Logging;

namespace Yugen.Domain.Common.Utils
{
  public static class LoggerExtensions
  {
    /// <summary>
    /// Extension method for consistently formatting window event logs.
    /// </summary>
    public static void LogWindowEvent<T>(
      this ILogger<T> logger,
      string message,
      Window window)
    {
      logger.LogDebug(
        "{Message}: {ProcessName} {ClassName}",
        message,
        window.ProcessName,
        window.ClassName
      );
    }
  }
}
