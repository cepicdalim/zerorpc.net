using ZeroRPC.NET.Common.Constants;

namespace ZeroRPC.NET.Common.Types.Configuration;

/// <summary>
/// The client configuration.
/// </summary>
public record ClientConfiguration
{
    /// <summary>
    /// The client identifier.
    /// </summary>
    public Guid Identifier { get; private set; } = Guid.NewGuid();
    /// <summary>
    /// The default timeout for the client.
    /// </summary>
    public TimeSpan DefaultTimeout { get; set; } = ConfigurationDefaults.DefaultTimeout;

    /// <summary>
    /// The connection configuration.
    /// </summary>
    public ConnectionConfiguration Connection { get; set; } = new();
}