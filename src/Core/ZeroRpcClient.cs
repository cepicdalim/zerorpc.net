using NetMQ;
using System.Collections.Concurrent;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using ZeroRPC.NET.Common.Attributes;
using ZeroRPC.NET.Common.Constants;
using ZeroRPC.NET.Common.Extensions;
using ZeroRPC.NET.Common.Types;
using ZeroRPC.NET.Common.Types.Dto;
using ZeroRPC.NET.Common.Types.Exceptions;
using ZeroRPC.NET.Factory;

namespace ZeroRPC.NET.Core;

/// <summary>
/// ZeroRPC client implementation.
/// </summary>
/// <typeparam name="T"></typeparam>
public class ZeroRpcClient<T> : DispatchProxy, IClient<T> where T : class
{
    private static TimeSpan _defaultTimeout = TimeSpan.FromSeconds(15);
    private static readonly ConcurrentDictionary<string, string> _clients = new();

    /// <summary>
    /// Configures the client by associating a type with a remote endpoint.
    /// </summary>
    /// <param name="target">The target type to associate with the endpoint.</param>
    /// <param name="endpoint">The remote endpoint URI.</param>
    /// <param name="defaultTimeout"></param>
    public static void InitializeClient(Type target, string endpoint, TimeSpan defaultTimeout)
    {
        _defaultTimeout = defaultTimeout;
        _clients[target.FullName] = endpoint;
    }

    /// <inheritdoc />
    protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
    {
        using var client = DealerFactory.CreateDealer(_clients[typeof(T).FullName]);
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
                rules?.Timeout ?? _defaultTimeout
            );

        client.SendRequest(request);

        if (targetMethod.ReturnType == typeof(void))
        {
            return null;
        }

        if (targetMethod.ReturnType == typeof(Task))
        {
            return Task.CompletedTask;
        }


        return HandleResponse(client, request);
    }

    private static object? HandleResponse(INetMQSocket client, RpcRequest request)
    {
        while (true)
        {
            var incomingMessage = new NetMQMessage();
            if (client.TryReceiveMultipartMessage(request.Timeout, ref incomingMessage))
            {
                if (incomingMessage[ClientFrameIndex.CorrelationId].ConvertToString() != request.CorrelationId)
                {
                    continue;
                }

                if (incomingMessage.FrameCount <= ClientFrameIndex.Error)
                {
                    var response = incomingMessage[ClientFrameIndex.Response].ConvertToString();
                    var result = JsonSerializer.Deserialize(response, request.ReturnType ?? typeof(void));
                    return result;
                }

                var deserializedException = JsonSerializer.Deserialize<ZeroRpcException>(incomingMessage[ClientFrameIndex.Error].ConvertToString());
                throw deserializedException ?? new ZeroRpcException();
            }

            client.Dispose();
            throw new ZeroRpcException($"Timout reached while waiting for response from {request.FullPath}");
        }
    }

    private static string GetRpcNamespace(MemberInfo? declaringType)
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