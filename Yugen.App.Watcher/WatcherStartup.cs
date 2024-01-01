using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yugen.Domain.Common;
using Yugen.Infrastructure.Common;
using static Yugen.Infrastructure.WindowsApi.WindowsApiService;

namespace Yugen.App.Watcher
{
  public sealed class WatcherStartup
  {
    /// <summary>
    /// Watcher is responsible for restoring all managed handles when the main process is
    /// killed unexpectedly.
    /// </summary>
    public static async Task<ExitCode> Run(int ipcServerPort)
    {
      var client = new IpcClient(ipcServerPort);
      var managedHandles = new List<IntPtr>();

      try
      {
        await client.ConnectAsync();

        // Get window handles that are initially managed on startup.
        foreach (var handle in await GetInitialHandles(client))
          managedHandles.Add(handle);

        // Subscribe to manage + unmanage window events.
        _ = await client.SendAndWaitReplyAsync(
          $"subscribe -e {DomainEvent.WindowManaged},{DomainEvent.WindowUnmanaged}"
        );

        // Continuously listen for manage + unmanage events.
        while (true)
        {
          var (isManaged, handle) = await GetManagedEvent(client);

          if (isManaged)
            managedHandles.Add(handle);
          else
            managedHandles.Remove(handle);
        }
      }
      catch (Exception)
      {
        // Restore managed handles on failure to communicate with the main process'
        // IPC server.
        RestoreHandles(managedHandles);
        await client.DisconnectAsync();
        return ExitCode.Success;
      }
    }

    /// <summary>
    /// Query for initial window handles via IPC server.
    /// </summary>
    private static async Task<IEnumerable<IntPtr>> GetInitialHandles(IpcClient client)
    {
      var response = await client.SendAndWaitReplyAsync("windows");

      return response
        .EnumerateArray()
        .Select(value => new IntPtr(value.GetInt64()));
    }

    /// <summary>
    /// Get window handles from managed and unmanaged window events.
    /// </summary>
    /// <returns>Tuple of whether the handle is managed, and the handle itself</returns>
    private static async Task<(bool, IntPtr)> GetManagedEvent(IpcClient client)
    {
      var response = await client.ReceiveAsync();

      return response.GetProperty("type").GetString() switch
      {
        DomainEvent.WindowManaged => (
          true,
          new IntPtr(response.GetProperty("managedWindow").GetProperty("handle").GetInt64())
        ),
        DomainEvent.WindowUnmanaged => (
          false,
          new IntPtr(response.GetProperty("unmanagedHandle").GetInt64())
        ),
        _ => throw new Exception("Received unrecognized event.")
      };
    }

    /// <summary>
    /// Restore given window handles.
    /// </summary>
    private static void RestoreHandles(List<IntPtr> handles)
    {
      foreach (var handle in handles)
        ShowWindowAsync(handle, ShowWindowFlags.ShowNoActivate);
    }
  }
}
