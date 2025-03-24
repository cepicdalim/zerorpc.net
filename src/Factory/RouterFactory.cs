using NetMQ;
using NetMQ.Sockets;

internal static class RouterFactory
{
    internal static NetMQSocket CreateRouter(int port, string host = "*")
    {
        var routerSocket = new RouterSocket();
        routerSocket.Bind($"tcp://{host}:{port}");
        routerSocket.Options.Linger = TimeSpan.Zero;
        routerSocket.Options.TcpKeepalive = true;
        routerSocket.Options.Identity = Guid.NewGuid().ToByteArray();
#if DEBUG
        Console.WriteLine($"ZeroRPC Server started at port {port}");
#endif
        return routerSocket;
    }
}