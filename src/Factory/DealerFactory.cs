using NetMQ;
using NetMQ.Sockets;

namespace ZeroRPC.NET.Factory;

internal static class DealerFactory
{
    internal static NetMQSocket CreateDealer(string endpoint)
    {
        var dealerSocket = new DealerSocket();
        dealerSocket.Connect(endpoint);
        dealerSocket.Options.Identity = Encoding.UTF8.GetBytes(Guid.NewGuid().ToString());
        dealerSocket.Options.Linger = TimeSpan.Zero;
        dealerSocket.Options.TcpKeepalive = true;
        return dealerSocket;
    }
}