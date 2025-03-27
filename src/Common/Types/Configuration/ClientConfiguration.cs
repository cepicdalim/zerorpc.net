using ZeroRPC.NET.Common.Constants;

namespace ZeroRPC.NET.Common.Types.Configuration;

/// <summary>
/// The client configuration.
/// </summary>
public record ClientConfiguration
{
    /// <summary>
    /// The default timeout for the client.
    /// </summary>
    public TimeSpan DefaultTimeout { get; set; } = ConfigurationDefaults.DefaultTimeout;

    /// <summary>
    /// The connection configuration.
    /// </summary>
    public ConnectionConfiguration Connection { get; set; } = new ConnectionConfiguration();
}