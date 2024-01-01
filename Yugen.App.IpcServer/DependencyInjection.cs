using Microsoft.Extensions.DependencyInjection;

namespace Yugen.App.IpcServer
{
  public static class DependencyInjection
  {
    public static IServiceCollection AddIpcServerServices(this IServiceCollection services)
    {
      services.AddSingleton<IpcMessageHandler>();
      services.AddSingleton<IpcServerStartup>();

      return services;
    }
  }
}
