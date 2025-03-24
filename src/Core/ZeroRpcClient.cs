using System.Reflection;
using System.Text.Json;
using NetMQ;
using System.Collections.Concurrent;

public class ZeroRpcClient<T> : DispatchProxy, IClient<T> where T : class
{
    private static TimeSpan DefaultTimeout = TimeSpan.FromSeconds(15);
    private static ConcurrentDictionary<string, string> clients = new();

    /// <summary>
    /// Configures the client by associating a type with a remote endpoint.
    /// </summary>
    /// <param name="target">The target type to associate with the endpoint.</param>
    /// <param name="endpoint">The remote endpoint URI.</param>
    public static void InitializeClient(Type target, string endpoint, TimeSpan defaultTimeout)
    {
        DefaultTimeout = defaultTimeout;
        clients[target.FullName] = endpoint;
    }

    protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
    {
        using var client = DealerFactory.CreateDealer(clients[typeof(T).FullName]);
        if (targetMethod == null)
        {
            throw new ArgumentException("Target method cannot be null.");
        }

        var rules = targetMethod.GetCustomAttribute<RemoteExecutionRule>();
        var rpcNamespace = GetRpcNamespace(targetMethod.DeclaringType);
        var fullPath = $"{rpcNamespace}.{targetMethod.Name}";
        var request =
        new RpcRequest(fullPath,
            SerializeArguments(args),
            targetMethod.ReturnType,
            rules?.RetryCount ?? 0,
            rules?.Timeout ?? DefaultTimeout
        );

        client.SendRequest(request);

        if (targetMethod.ReturnType != typeof(void) && targetMethod.ReturnType != typeof(Task))
        {
            return HandleResponse(client, request);
        }

        return null;
    }

    private object? HandleResponse(NetMQSocket client, RpcRequest request)
    {
        var incomingMessage = new NetMQMessage();
        if (client.TryReceiveMultipartMessage(request.Timeout, ref incomingMessage))
        {
            if (incomingMessage[ClientFrameIndex.CorrelationId].ConvertToString() != request.CorrelationId)
            {
                return HandleResponse(client, request);
            }

            if (incomingMessage.FrameCount <= ClientFrameIndex.Error)
            {
                string response = incomingMessage[ClientFrameIndex.Response].ConvertToString();
                var result = JsonSerializer.Deserialize(response, request.ReturnType ?? typeof(void));
                return result;
            }
            else
            {
                var deserializedException = JsonSerializer.Deserialize<ZeroRpcException>(incomingMessage[ClientFrameIndex.Error].ConvertToString());
                throw deserializedException ?? new ZeroRpcException();
            }
        }
        client.Dispose();
        throw new ZeroRpcException($"Timout reached while waiting for response from {request.FullPath}");
    }

    private static string GetRpcNamespace(Type? declaringType)
    {
        var namespaceAttribute = declaringType?.GetCustomAttribute<RemoteService>()
            ?? throw new ArgumentException($"Missing RemoteNamespace attribute on {typeof(T).Name}");

        return $"{namespaceAttribute.Namespace}.{namespaceAttribute.Name}".TrimStart('.');
    }

    private static string SerializeArguments(object?[]? args)
    {
        return JsonSerializer.Serialize(args ?? Array.Empty<object>());
    }
}
