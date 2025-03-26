using System.Threading.Tasks;

namespace ZeroRPC.NET.Common.Types.Dto;

/// <summary>
/// Represents a request to be sent to a remote endpoint.
/// </summary>
public record RpcRequest
{
    /// <summary>
    /// Gets the correlation identifier for the request.
    /// </summary>
    public string CorrelationId { get; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Gets the full path of the method to be invoked.
    /// </summary>
    public string? FullPath { get; set; }

    /// <summary>
    /// Gets the serialized arguments for the method to be invoked.
    /// </summary>
    public string? SerializedArgs { get; set; }

    /// <summary>
    /// Gets the return type of the method to be invoked.
    /// </summary>
    public Type? ReturnType { get; set; }

    /// <summary>
    /// Gets the timeout for the request.
    /// </summary>
    public TimeSpan Timeout { get; set; }

    /// <summary>
    /// Gets the host for the request.
    /// </summary>
    public string? Host { get; set; }

    /// <summary>
    /// Gets the task completion source for the request.
    /// </summary>
    public TaskCompletionSource<object?> TaskCompletionSource { get; set; } = new TaskCompletionSource<object?>();
}