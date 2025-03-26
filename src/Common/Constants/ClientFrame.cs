namespace ZeroRPC.NET.Common.Constants;

/// <summary>
/// Represents the indices of the elements in a client frame.
/// </summary>
public static class ClientFrameIndex
{
    /// <summary>
    /// Gets the index of the correlation identifier in the client frame.
    /// </summary>
    public const int CorrelationId = 0;

    /// <summary>
    /// Gets the index of the full path in the client frame.
    /// </summary>
    public const int Response = 1;

    /// <summary>
    /// Gets the index of the serialized arguments in the client frame.
    /// </summary>
    public const int Error = 2;
}