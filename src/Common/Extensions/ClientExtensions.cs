using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using NetMQ;
using ZeroRPC.NET.Common.Types.Dto;
using ZeroRPC.NET.Core;

namespace ZeroRPC.NET.Common.Extensions;

/// <summary>
/// Extension methods for the client.
/// </summary>
public static class ClientExtensions
{
    /// <summary>
    /// Adds a ZeroRPC client to the service collection.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="port"></param>
    /// <param name="defaultTimeout"></param>
    /// <param name="host"></param>
    /// <typeparam name="TInterface"></typeparam>
    /// <returns></returns>
    public static IServiceCollection AddZeroRpcClient<TInterface>(this IServiceCollection services, int port, TimeSpan? defaultTimeout = null, string host = "127.0.0.1")
        where TInterface : class
    {
        services.AddSingleton(provider =>
        {
            ZeroRpcClient<TInterface>.InitializeClient(typeof(TInterface), $"tcp://{host}:{port}", defaultTimeout ?? TimeSpan.FromSeconds(15));
            return DispatchProxy.Create<TInterface, ZeroRpcClient<TInterface>>();
        });

        return services;
    }

    /// <summary>
    /// Sends a request to the server.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="request"></param>
    public static void SendRequest(this NetMQSocket client, RpcRequest request)
    {
        var message = new NetMQMessage();
        message.Append(request.CorrelationId);
        message.Append(request.FullPath);
        message.Append(request.SerializedArgs);
        message.Append(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + (request.Timeout.Ticks / 10_000));
        client.SendMultipartMessage(message);
    }
}