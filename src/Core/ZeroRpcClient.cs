using NetMQ;
using System.Collections.Concurrent;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using ZeroRPC.NET.Common.Attributes;
using ZeroRPC.NET.Common.Constants;
using ZeroRPC.NET.Common.Extensions;
using ZeroRPC.NET.Common.Types;
using ZeroRPC.NET.Common.Types.Configuration;
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
    private static readonly ConcurrentDictionary<string, ClientConfiguration> _configurations = new();
    private static readonly ConcurrentDictionary<string, RpcRequest> _requests = new();
    private static readonly ConcurrentDictionary<string, NetMQSocket> _clients = [];
    private static readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web);

    /// <summary>
    /// Initializes the client.
    /// </summary>
    /// <param name="clientConfiguration"></param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public static void InitializeClient(ClientConfiguration clientConfiguration)
    {
        var connectionString = clientConfiguration.Connection.ToString();
        if (!_clients.ContainsKey(connectionString))
        {
            var client = DealerFactory.CreateDealer(clientConfiguration.Connection);
            _clients[connectionString] = client;

            var poller = new NetMQPoller { client };
            client.ReceiveReady += OnResponseReceived;
            poller.RunAsync();
        }

        _configurations[typeof(T).FullName] = clientConfiguration;
    }

    private static void OnResponseReceived(object sender, NetMQSocketEventArgs e)
    {
        var incomingMessage = e.Socket.ReceiveMultipartMessage();
        var correlationId = incomingMessage[ClientFrameIndex.CorrelationId].ConvertToString();
        if (!_requests.TryRemove(correlationId, out var request)) return;

        try
        {
            if (incomingMessage.FrameCount <= ClientFrameIndex.Error)
            {
                var response = incomingMessage[ClientFrameIndex.Response].ConvertToString();

                if (request.ReturnType?.IsGenericType == true && request.ReturnType.GetGenericTypeDefinition() == typeof(Task<>))
                {
                    var result = JsonSerializer.Deserialize(response, request.ReturnType.GetGenericArguments()[0], _jsonOptions);
                    request.TaskCompletionSource.SetResult(result);
                }
                else if (request.ReturnType == typeof(Task))
                {
                    request.TaskCompletionSource.SetResult(Task.CompletedTask);
                }
                else if (request.ReturnType == typeof(void))
                {
                    request.TaskCompletionSource.SetResult(null);
                }
                else
                {
                    var result = JsonSerializer.Deserialize(response, request.ReturnType ?? typeof(void), _jsonOptions);
                    request.TaskCompletionSource.SetResult(result);
                }
            }
            else
            {
                var deserializedException = JsonSerializer.Deserialize<ZeroRpcException>(incomingMessage[ClientFrameIndex.Error].ConvertToString(), _jsonOptions);
                request.TaskCompletionSource.SetException(deserializedException);
            }
        }
        catch (Exception ex)
        {
            request.TaskCompletionSource.SetException(new ZeroRpcException(ex));
        }
    }


    /// <inheritdoc />
    protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
    {
        if (targetMethod == null) return null;

        var clientConfiguration = _configurations[typeof(T).FullName];
        var rpcNamespace = GetRpcService(targetMethod.DeclaringType);
        var timeout = targetMethod.GetCustomAttribute<RemoteExecutionRule>()?.Timeout ?? clientConfiguration.DefaultTimeout;

        var request = PrepareRequest(targetMethod, args, clientConfiguration, rpcNamespace, timeout);
        RegisterCancellationToken(request);

        SendRequest(request);
        return HandleResponse(request);
    }

    private static RpcRequest PrepareRequest(MethodInfo targetMethod, object?[]? args, ClientConfiguration clientConfiguration, string rpcNamespace, TimeSpan timeout)
    {
        return new RpcRequest(clientConfiguration.Connection.ToString(),
            $"{rpcNamespace}.{targetMethod.Name}",
            SerializeArguments(args),
            targetMethod.ReturnType,
            timeout,
            new CancellationTokenSource(timeout));
    }

    private static void RegisterCancellationToken(RpcRequest request)
    {
        request.CancellationTokenSource.Token.Register(() =>
        {
            if (_requests.TryRemove(request.CorrelationId, out var rpcRequest))
            {
                rpcRequest.TaskCompletionSource.TrySetException(new TimeoutException($"Request timed out after {request.Timeout.TotalMilliseconds} ms"));
            }
        });
    }

    private object? HandleResponse(RpcRequest request)
    {
        return request.ReturnType switch
        {
            { IsGenericType: true } when request.ReturnType.GetGenericTypeDefinition() == typeof(Task<>) => GetTaskResult(request),
            not null when request.ReturnType == typeof(Task) => Task.CompletedTask,
            not null when request.ReturnType == typeof(void) => null,
            _ => request.TaskCompletionSource.Task.GetAwaiter().GetResult()
        };
    }

    private object? GetTaskResult(RpcRequest request)
    {
        if (request.ReturnType == null) return null;

        var genericType = request.ReturnType.GetGenericArguments()[0];

        var originalTcs = request.TaskCompletionSource;

        var newTcsType = typeof(TaskCompletionSource<>).MakeGenericType(genericType);
        var newTcs = Activator.CreateInstance(newTcsType);

        var taskProperty = newTcsType.GetProperty("Task");
        var setResultMethod = newTcsType.GetMethod("SetResult");
        var setExceptionMethod = newTcsType.GetMethod("SetException", [typeof(Exception)]);
        var setCanceledMethod = newTcsType.GetMethod("SetCanceled", []);

        var originalTask = originalTcs.Task;
        originalTask.ContinueWith((task) =>
        {
            try
            {
                if (task.IsFaulted)
                {
                    setExceptionMethod?.Invoke(newTcs, [new ZeroRpcException(task.Exception)]);
                }
                else if (task.IsCanceled)
                {
                    setCanceledMethod?.Invoke(newTcs, null);
                }
                else
                {
                    var result = Convert.ChangeType(task.Result, genericType);
                    setResultMethod?.Invoke(newTcs, [result]);
                }
            }
            catch (Exception ex)
            {
                setExceptionMethod?.Invoke(newTcs, [new ZeroRpcException(ex)]);
            }
        });
        return taskProperty?.GetValue(newTcs);
    }


    private void SendRequest(RpcRequest requests)
    {
        if (requests.Host == null)
            throw new ArgumentException("Host cannot be null.");

        _clients[requests.Host].SendRequest(requests);
        _requests.TryAdd(requests.CorrelationId, requests);
    }

    private static string GetRpcService(MemberInfo? declaringType)
    {
        var namespaceAttribute = declaringType?.GetCustomAttribute<RemoteService>()
                                 ?? throw new ArgumentException($"Missing RemoteService attribute on {typeof(T).Name}");

        return $"{namespaceAttribute.Namespace}.{namespaceAttribute.Name}".TrimStart('.');
    }

    private static string SerializeArguments(object?[]? args)
    {
        return JsonSerializer.Serialize(args ?? Array.Empty<object>(), _jsonOptions);
    }
}