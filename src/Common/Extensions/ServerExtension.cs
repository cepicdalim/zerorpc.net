
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using NetMQ;

public static class ServerExtensions
{
    public static IServiceCollection AddZeroRpcServer(this IServiceCollection services)
    {
        services.AddSingleton<ZeroRpcServer>();
        return services;
    }

    public static IServiceProvider RegisterZeroRpcServices(this IServiceProvider serviceProvider, int port, CancellationToken cancellationToken = default)
    {
        var server = serviceProvider.GetRequiredService<ZeroRpcServer>();
        server.RegisterServices(port, cancellationToken);
        return serviceProvider;
    }

    public static void SendOkResponse(this NetMQSocket server, string correlationId, byte[] receiverIdentity, string payload)
    {
        var message = new NetMQMessage();
        message.Append(receiverIdentity);
        message.Append(correlationId);
        message.Append(payload);
        server.SendMultipartMessage(message);
    }

    public static void SendErrorResponse(this NetMQSocket server, string correlationId, byte[] receiverIdentity, ZeroRpcException error)
    {
        var message = new NetMQMessage();
        message.Append(receiverIdentity);
        message.Append(correlationId);
        message.AppendEmptyFrame(); // Empty frame for payload
        message.Append(JsonSerializer.Serialize(error));
        server.SendMultipartMessage(message);
    }
}