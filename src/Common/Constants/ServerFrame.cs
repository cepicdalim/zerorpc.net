namespace ZeroRPC.NET.Common.Constants;

/// <summary>
/// Represents the indices of the elements in a server frame.
/// </summary>
public static class ServerFrameIndex
{
    /// <summary>
    /// Gets the index of the identity in the server frame.
    /// </summary>
    public const int Identity = 0;

    /// <summary>
    /// Gets the index of the correlation identifier in the server frame.
    /// </summary>
    public const int CorrelationId = 1;

    /// <summary>
    /// Gets the index of the payload in the server frame.
    /// </summary>
    public const int Payload = 2;

    /// <summary>
    /// Gets the index of the parameters in the server frame.
    /// </summary>
    public const int Params = 3;

    /// <summary>
    /// Gets the index of the TTL in the server frame.
    /// </summary>
    public const int Ttl = 4;
}