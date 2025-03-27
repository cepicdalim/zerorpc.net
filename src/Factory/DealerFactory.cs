using NetMQ;
using NetMQ.Sockets;
using ZeroRPC.NET.Common.Types.Configuration;

namespace ZeroRPC.NET.Factory;

internal static class DealerFactory
{
    internal static NetMQSocket CreateDealer(ConnectionConfiguration connectionConfiguration)
    {
        var dealerSocket = new DealerSocket();
        dealerSocket.Connect(connectionConfiguration.ToString());
        dealerSocket.Options.Identity = Encoding.UTF8.GetBytes(Guid.NewGuid().ToString());
        dealerSocket.Options.Linger = TimeSpan.Zero;
        dealerSocket.Options.TcpKeepalive = true;
        return dealerSocket;
    }
}