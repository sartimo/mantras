using System;
using Vostok.Sys.Metrics.PerfCounters;

namespace Yugen.Infrastructure.WindowsApi
{
  /// <summary>
  /// Provides access to current CPU statistics.
  /// </summary>
  public class CpuStatsService : IDisposable
  {
    private readonly IPerformanceCounter<double> _cpuCounter =
      PerformanceCounterFactory.Default.CreateCounter(
        "Processor Information",
        "% Processor Utility",
        "_Total"
      );

    /// <inheritdoc />
    ~CpuStatsService() => Dispose();

    /// <inheritdoc />
    public void Dispose()
    {
      _cpuCounter.Dispose();
      GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Returns the current CPU utilization as a percentage.
    /// </summary>
    public double GetCpuUsage()
    {
      try
      {
        return _cpuCounter.Observe();
      }
      catch
      {
        return 0;
      }
    }
  }
}
