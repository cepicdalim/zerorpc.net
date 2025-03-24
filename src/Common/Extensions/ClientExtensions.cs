
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using NetMQ;

public static class ClientExtensions
{
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