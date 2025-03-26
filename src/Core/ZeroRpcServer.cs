using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using NetMQ;
using ZeroRPC.NET.Common.Attributes;
using ZeroRPC.NET.Common.Constants;
using ZeroRPC.NET.Common.Types;
using ZeroRPC.NET.Common.Types.Exceptions;
using ZeroRPC.NET.Factory;

namespace ZeroRPC.NET.Core;

/// <summary>
/// ZeroRPC server implementation.
/// </summary>
public class ZeroRpcServer : IServer
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ConcurrentDictionary<string, MethodInfo> _services = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="ZeroRpcServer"/> class.
    /// </summary>
    /// <param name="serviceProvider"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public ZeroRpcServer(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    /// <summary>
    /// Register services to be exposed via ZeroRPC.
    /// </summary>
    /// <param name="port"></param>
    /// <param name="cancellationToken"></param>
    public void RegisterServices(int port, CancellationToken cancellationToken = default)
    {
        CollectRemoteServices();
        var runtime = new NetMQRuntime();
        runtime.Run(cancellationToken, StartServer(port, cancellationToken));
    }

    private void CollectRemoteServices()
    {
        var types = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .ToList();

        foreach (var type in types)
        {
            var instance = _serviceProvider.GetService(type);
            if (instance == null) continue;

            var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance);

            foreach (var method in methods)
            {
                var attribute = method.GetCustomAttribute<RemoteMethod>();
                if (attribute == null) continue;

                var methodKey = $"{type.FullName}.{attribute.Name}".TrimStart('.');
#if DEBUG
                Console.WriteLine($"Registering method: {methodKey}");
#endif
                _services.TryAdd(methodKey, method);
            }
        }
    }

    private async Task StartServer(int port, CancellationToken cancellationToken)
    {
        var routerSocket = RouterFactory.CreateRouter(port);

        while (!cancellationToken.IsCancellationRequested)
        {
            var incomingMessage = await routerSocket.ReceiveMultipartMessageAsync(cancellationToken: cancellationToken);
            ProcessRequest(routerSocket, incomingMessage);
        }
    }

    private void ProcessRequest(NetMQSocket routerSocket, NetMQMessage incomingMessage)
    {
        var correlationId = incomingMessage[ServerFrameIndex.CorrelationId].ConvertToString();
        var clientIdentity = incomingMessage[ServerFrameIndex.Identity].ToByteArray();
        var method = incomingMessage[ServerFrameIndex.Payload].ConvertToString();
        var ttl = incomingMessage[ServerFrameIndex.Ttl].ConvertToInt64();

        if (DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() > ttl)
        {
            Console.WriteLine($"Request with CID: {correlationId} has timed out, ignored. TTL: {ttl}");
            return;
        }

        try
        {
            if (!_services.TryGetValue(method, out var methodInfo))
            {
                throw new ZeroRpcException($"Method '{method}' not found.");
            }

            var args = DeserializeArguments(incomingMessage, methodInfo);
            var serviceInstance = _serviceProvider.GetService(methodInfo.DeclaringType!)
                                  ?? throw new ZeroRpcException($"Service for type '{methodInfo.DeclaringType!.Name}' not found.");

            var result = methodInfo.Invoke(serviceInstance, args);

            if (methodInfo.ReturnType != typeof(void) && methodInfo.ReturnType != typeof(Task))
            {
                routerSocket.SendOkResponse(correlationId, clientIdentity, JsonSerializer.Serialize(result));
            }
        }
        catch (TargetInvocationException ex)
        {
            routerSocket.SendErrorResponse(correlationId, clientIdentity, new ZeroRpcException(ex.InnerException));
        }
        catch (Exception ex)
        {
            routerSocket.SendErrorResponse(correlationId, clientIdentity, new ZeroRpcException(ex));
        }
    }

    private static object?[] DeserializeArguments(NetMQMessage incomingMessage, MethodInfo methodInfo)
    {
        var parameterTypes = methodInfo.GetParameters().Select(p => p.ParameterType).ToArray();
        var argsJson = incomingMessage[ServerFrameIndex.Params].ConvertToString();
        var args = JsonSerializer.Deserialize<object[]>(argsJson) ?? Array.Empty<object>();

        if (args.Length != parameterTypes.Length)
        {
            throw new ZeroRpcException($"Parameter count mismatch for method '{methodInfo.Name}'.");
        }

        return args.Select((arg, index) =>
        {
            if (arg is JsonElement jsonElement)
            {
                return JsonSerializer.Deserialize(jsonElement.GetRawText(), parameterTypes[index]);
            }

            return Convert.ChangeType(arg, parameterTypes[index]);
        }).ToArray();
    }
}