using NetMQ;
using ZeroRPC.NET.Common.Types.Exceptions;

/// <summary>
/// Extension methods for NetMQMessage.
/// </summary>
public static class NetMQMessageExtensions
{
    /// <summary>
    /// Sends a ZeroRPC request using the specified NetMQSocket client.
    /// </summary>
    /// <param name="client">The NetMQSocket client used to send the message.</param>
    /// <param name="message">The NetMQMessage to send.</param>
    /// <remarks>
    /// This method constructs a multipart NetMQMessage and sends it using the provided client socket.
    /// </remarks>
    public static void SendZeroRpcRequest(this NetMQSocket client, NetMQMessage message)
    {
        client.SendMultipartMessage(message);
    }
}