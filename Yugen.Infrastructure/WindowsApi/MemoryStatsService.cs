using System;
using Microsoft.VisualBasic.Devices;
using Vostok.Sys.Metrics.PerfCounters;

namespace Yugen.Infrastructure.WindowsApi
{
  /// <summary>
  /// Provides access to current memory statistics.
  /// </summary>
  public class MemoryStatsService : IDisposable
  {
    private readonly IPerformanceCounter<double> _availableBytes =
      PerformanceCounterFactory.Default.CreateCounter("Memory", "Available Bytes");

    private readonly long _physicalBytes = (long)new ComputerInfo().TotalPhysicalMemory;

    /// <inheritdoc />
    ~MemoryStatsService() => Dispose();

    /// <inheritdoc />
    public void Dispose()
    {
      GC.SuppressFinalize(this);
      _availableBytes.Dispose();
    }

    /// <summary>
    /// Returns the current memory utilization as a percentage.
    /// </summary>
    public double GetMemoryUsage()
    {
      try
      {
        var percent = (_physicalBytes - _availableBytes.Observe()) / _physicalBytes;
        return percent * 100;
      }
      catch
      {
        return 0;
      }
    }
  }
}
