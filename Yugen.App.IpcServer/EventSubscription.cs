using System;
using System.Net.WebSockets;

namespace Yugen.App.IpcServer
{
  public sealed record EventSubscription(Guid SubscriptionId, WebSocket WebSocket);
}
