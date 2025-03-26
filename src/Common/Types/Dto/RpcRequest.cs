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
    public string FullPath { get; }

    /// <summary>
    /// Gets the serialized arguments for the method to be invoked.
    /// </summary>
    public string SerializedArgs { get; }

    /// <summary>
    /// Gets the return type of the method to be invoked.
    /// </summary>
    public Type? ReturnType { get; }

    /// <summary>
    /// Gets the timeout for the request.
    /// </summary>
    public TimeSpan Timeout { get; set; }


    /// <summary>
    /// Initializes a new instance of the <see cref="RpcRequest"/> class.
    /// </summary>
    /// <param name="fullPath"></param>
    /// <param name="serializedArgs"></param>
    /// <param name="returnType"></param>
    /// <param name="timeout"></param>
    public RpcRequest(string fullPath, string serializedArgs, Type? returnType, TimeSpan timeout)
    {
        FullPath = fullPath;
        SerializedArgs = serializedArgs;
        ReturnType = returnType;
        Timeout = timeout;
    }
}