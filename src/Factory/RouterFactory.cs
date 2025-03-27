using NetMQ;
using NetMQ.Sockets;
using ZeroRPC.NET.Common.Types.Configuration;

namespace ZeroRPC.NET.Factory;

internal static class RouterFactory
{
    internal static NetMQSocket CreateRouter(ConnectionConfiguration connectionConfiguration)
    {
        var routerSocket = new RouterSocket();
        routerSocket.Bind(connectionConfiguration.ToString());
        routerSocket.Options.Linger = TimeSpan.Zero;
        routerSocket.Options.TcpKeepalive = true;
        routerSocket.Options.Identity = Guid.NewGuid().ToByteArray();
#if DEBUG
        Console.WriteLine($"ZeroRPC Server started at port {connectionConfiguration}");
#endif
        return routerSocket;
    }
}