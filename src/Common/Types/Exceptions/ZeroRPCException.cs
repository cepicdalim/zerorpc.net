namespace ZeroRPC.NET.Common.Types.Exceptions;

/// <summary>
/// Represents an exception that occurred during a ZeroRPC operation.
/// </summary>
[Serializable]
public class ZeroRpcException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ZeroRpcException"/> class.
    /// </summary>
    public ZeroRpcException()
    {
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="ZeroRpcException"/> class.
    /// </summary>
    /// <param name="ex"></param>
    public ZeroRpcException(Exception? ex)
    {
        _Message = ex?.Message ?? "Unknown error occurred.";
        _StackTrace = ex?.StackTrace;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ZeroRpcException"/> class.
    /// </summary>
    /// <param name="pMessage"></param>
    public ZeroRpcException(string pMessage)
    {
        _Message = pMessage;
    }

    /// <inheritdoc />
    public override string StackTrace => _StackTrace ?? "";

    /// <inheritdoc />
    public override string Message => _Message ?? "Unknown error";

    /// <summary>
    /// Gets or sets the stack trace of the exception.
    /// </summary>
    public string? _StackTrace { get; set; }

    /// <summary>
    /// Gets or sets the message of the exception.
    /// </summary>
    public string? _Message { get; set; }
}