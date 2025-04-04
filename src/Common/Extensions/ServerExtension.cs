using NetMQ;
using System.Text.Json;
using ZeroRPC.NET.Core;
using ZeroRPC.NET.Common.Types.Exceptions;
using Microsoft.Extensions.DependencyInjection;


namespace ZeroRPC.NET.Common.Extensions;

/// <summary>
/// Extension methods for the server.
/// </summary>
public static class ServerExtensions
{
    /// <summary>
    /// Adds a ZeroRPC server to the service collection.
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddZeroRpcServer(this IServiceCollection services)
    {
        services.AddSingleton<ZeroRpcServer>();
        return services;
    }

    /// <summary>
    /// Sends an OK response to the client.
    /// </summary>
    /// <param name="server"></param>
    /// <param name="correlationId"></param>
    /// <param name="receiverIdentity"></param>
    /// <param name="payload"></param>
    public static void SendOkResponse(this NetMQSocket server, string correlationId, byte[] receiverIdentity, string payload)
    {
        var message = new NetMQMessage();
        message.Append(receiverIdentity);
        message.Append(correlationId);
        message.Append(payload);
        server.SendZeroRpcRequest(message);
    }

    /// <summary>
    /// Sends an error response to the client.
    /// </summary>
    /// <param name="server"></param>
    /// <param name="correlationId"></param>
    /// <param name="receiverIdentity"></param>
    /// <param name="error"></param>
    public static void SendErrorResponse(this NetMQSocket server, string correlationId, byte[] receiverIdentity, ZeroRpcException error)
    {
        var message = new NetMQMessage();
        message.Append(receiverIdentity);
        message.Append(correlationId);
        message.AppendEmptyFrame(); // Empty frame for payload
        message.Append(JsonSerializer.Serialize(error));
        server.SendZeroRpcRequest(message);
    }
}