using ZeroRPC.NET.Common.Constants;

namespace ZeroRPC.NET.Common.Types.Configuration;

/// <summary>
/// The connection configuration.
/// </summary>
public record ConnectionConfiguration
{
    /// <summary>
    /// The host of the server.
    /// </summary>
    public string Host { get; set; } = ConfigurationDefaults.DefaultHost;

    /// <summary>
    /// The port of the server.
    /// </summary>
    public int Port { get; set; } = ConfigurationDefaults.DefaultPort;

    /// <summary>
    /// Type of connection.
    /// </summary>
    /// <remarks>
    /// The default value is <see cref="ProtocolType.Tcp"/>.
    /// </remarks>
    /// <seealso cref="Protocol"/>
    public ProtocolType Protocol { get; set; } = ProtocolType.Tcp;

    /// <summary>
    /// Get the connection string. 
    /// </summary>
    public override string ToString()
    {
        return $"{Protocol.ToString().ToLower()}://{Host}:{Port}";
    }
}
