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
    private static readonly ConcurrentDictionary<string, string> _hosts = new();
    private static readonly ConcurrentDictionary<string, RpcRequest> _requests = new();
    private static readonly ConcurrentDictionary<string, NetMQSocket> _clients = [];


    private static void Run(NetMQSocket client)
    {
        while (true)
        {
            var incomingMessage = client.ReceiveMultipartMessage();

            var correlationId = incomingMessage[ClientFrameIndex.CorrelationId].ConvertToString();
            if (_requests.TryRemove(correlationId, out var request))
            {
                if (incomingMessage.FrameCount <= ClientFrameIndex.Error)
                {
                    var response = incomingMessage[ClientFrameIndex.Response].ConvertToString();

                    // Deserialize the response respect to Task<T> or Task

                    if (request.ReturnType?.IsGenericType == true && request.ReturnType.GetGenericTypeDefinition() == typeof(Task<>))
                    {
                        var result = JsonSerializer.Deserialize(response, request.ReturnType.GetGenericArguments()[0]);
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
                        var result = JsonSerializer.Deserialize(response, request.ReturnType ?? typeof(void));
                        request.TaskCompletionSource.SetResult(result);
                    }
                }
                else
                {
                    var deserializedException = JsonSerializer.Deserialize<ZeroRpcException>(incomingMessage[ClientFrameIndex.Error].ConvertToString());
                    request.TaskCompletionSource.SetException(deserializedException);
                }

            }
        }
    }

    private void SendRequest(RpcRequest requests)
    {
        if (requests.Host == null)
            throw new ArgumentException("Host cannot be null.");

        _clients[requests.Host].SendRequest(requests);
        _requests.TryAdd(requests.CorrelationId, requests);
    }


    /// <summary>
    /// Configures the client by associating a type with a remote endpoint.
    /// </summary>
    /// <param name="target">The target type to associate with the endpoint.</param>
    /// <param name="endpoint">The remote endpoint URI.</param>
    /// <param name="defaultTimeout"></param>
    public static void InitializeClient(Type target, string endpoint, TimeSpan defaultTimeout)
    {
        _defaultTimeout = defaultTimeout;
        AddClient(endpoint);
    }

    private static void AddClient(string host)
    {
        if (!_clients.ContainsKey(host))
            _clients[host] = DealerFactory.CreateDealer(host);

        _hosts[typeof(T).FullName] = host;

        new TaskFactory().StartNew(() => Run(_clients[host]), TaskCreationOptions.LongRunning);
    }


    /// <inheritdoc />
    protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
    {
        if (targetMethod == null)
        {
            throw new ArgumentException("Target method cannot be null.");
        }

        var rules = targetMethod.GetCustomAttribute<RemoteExecutionRule>();
        var rpcNamespace = GetRpcNamespace(targetMethod.DeclaringType);
        var fullPath = $"{rpcNamespace}.{targetMethod.Name}";

        var request =
            new RpcRequest
            {
                FullPath = fullPath,
                SerializedArgs = SerializeArguments(args),
                ReturnType = targetMethod.ReturnType,
                Timeout = rules?.Timeout ?? _defaultTimeout,
                Host = _hosts[typeof(T).FullName],
            };

        SendRequest(request);

        return HandleResponse(request);
    }

    private object? HandleResponse(RpcRequest request)
    {
        // Handle void methods
        if (request.ReturnType == typeof(void))
        {
            return null;
        }

        // Handle Task (non-generic async method)
        if (request.ReturnType == typeof(Task))
        {
            return Task.CompletedTask;
        }

        // Handle Task<T> (generic async method)
        if (request.ReturnType?.IsGenericType == true && request.ReturnType.GetGenericTypeDefinition() == typeof(Task<>))
        {
            return GetTaskResult(request);
        }

        // Handle synchronous methods
        return request.TaskCompletionSource.Task.GetAwaiter().GetResult();
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
        originalTask.ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                setExceptionMethod?.Invoke(newTcs, [task.Exception!]);
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
        });
        return taskProperty?.GetValue(newTcs);
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