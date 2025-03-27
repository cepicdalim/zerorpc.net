using NetMQ;
using ZeroRPC.NET.Core;
using System.Reflection;
using ZeroRPC.NET.Common.Types.Dto;
using ZeroRPC.NET.Common.Types.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
    /// <param name="clientConfiguration"></param>
    /// <typeparam name="TInterface"></typeparam>
    /// <returns></returns>
    public static IServiceCollection AddZeroRpcClient<TInterface>(this IServiceCollection services, ClientConfiguration clientConfiguration)
        where TInterface : class
    {
        services.AddSingleton(provider =>
        {
            ZeroRpcClient<TInterface>.InitializeClient(clientConfiguration);
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
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        if (request.FullPath == null)
        {
            throw new ArgumentNullException(nameof(request.FullPath));
        }

        if (request.SerializedArgs == null)
        {
            throw new ArgumentNullException(nameof(request.SerializedArgs));
        }

        var message = new NetMQMessage();
        message.Append(request.CorrelationId);
        message.Append(request.FullPath);
        message.Append(request.SerializedArgs);
        message.Append(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + (request.Timeout.Ticks / 10_000));
        client.SendZeroRpcRequest(message);
    }
}