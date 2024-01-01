using Yugen.Infrastructure.Bussing;
using Yugen.Infrastructure.Common.CommandHandlers;
using Yugen.Infrastructure.Common.Commands;
using Yugen.Infrastructure.WindowsApi;
using Microsoft.Extensions.DependencyInjection;

namespace Yugen.Infrastructure
{
  public static class DependencyInjection
  {
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
      services.AddSingleton<Bus>();
      services.AddSingleton<KeybindingService>();
      services.AddSingleton<WindowEventService>();
      services.AddSingleton<CpuStatsService>();
      services.AddSingleton<GpuStatsService>();
      services.AddSingleton<MemoryStatsService>();
      services.AddSingleton<SystemVolumeInformation>();

      services.AddSingleton<ICommandHandler<ExitApplicationCommand>, ExitApplicationHandler>();
      services.AddSingleton<ICommandHandler<HandleFatalExceptionCommand>, HandleFatalExceptionHandler>();
      services.AddSingleton<ICommandHandler<NoopCommand>, NoopHandler>();

      // TODO: Change WindowsApiService to be compatible with DI.

      return services;
    }
  }
}
