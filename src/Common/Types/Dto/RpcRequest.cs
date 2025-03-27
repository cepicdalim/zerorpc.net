using System.Threading.Tasks;
using ZeroRPC.NET.Common.Constants;

namespace ZeroRPC.NET.Common.Types.Dto;

/// <summary>
/// Represents a request to be sent to a remote endpoint.
/// </summary>
public record RpcRequest
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RpcRequest"/> struct.
    /// </summary>
    /// <param name="host"></param>
    /// <param name="fullPath"></param>
    /// <param name="serializedArgs"></param>
    /// <param name="returnType"></param>
    /// <param name="timeout"></param>
    /// <param name="cancellationTokenSource"></param>
    public RpcRequest(string host, string fullPath, string serializedArgs, Type returnType, TimeSpan timeout, CancellationTokenSource cancellationTokenSource)
    {
        FullPath = fullPath;
        SerializedArgs = serializedArgs;
        ReturnType = returnType;
        Timeout = timeout;
        CancellationTokenSource = cancellationTokenSource;
        Host = host;
        TaskCompletionSource = new TaskCompletionSource<object?>();
    }

    /// <summary>
    /// Gets the correlation identifier for the request.
    /// </summary>
    public string CorrelationId { get; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Gets the full path of the method to be invoked.
    /// </summary>
    public string? FullPath { get; }

    /// <summary>
    /// Gets the serialized arguments for the method to be invoked.
    /// </summary>
    public string? SerializedArgs { get; }

    /// <summary>
    /// Gets the return type of the method to be invoked.
    /// </summary>
    public Type? ReturnType { get; }

    /// <summary>
    /// Gets the timeout for the request.
    /// </summary>
    public TimeSpan Timeout { get; } = ConfigurationDefaults.DefaultTimeout;

    /// <summary>
    /// Gets the cancellation token source for the request.
    /// </summary>
    public CancellationTokenSource CancellationTokenSource { get; }

    /// <summary>
    /// Gets the host for the request.
    /// </summary>
    public string? Host { get; }

    /// <summary>
    /// Gets the task completion source for the request.
    /// </summary>
    public TaskCompletionSource<object?> TaskCompletionSource { get; } = new();
}