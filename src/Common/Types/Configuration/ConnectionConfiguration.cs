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
    public string Host { get; }

    /// <summary>
    /// The port of the server.
    /// </summary>
    public int Port { get; }

    /// <summary>
    /// Type of connection.
    /// </summary>
    /// <remarks>
    /// The default value is <see cref="ProtocolType.Tcp"/>.
    /// </remarks>
    public ProtocolType Protocol { get; }

    /// <summary>
    /// Get the connection string. 
    /// </summary>
    public override string ToString()
    {
        return $"{Protocol.ToString().ToLower()}://{Host}:{Port}";
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConnectionConfiguration"/> class.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"></exception>
    public ConnectionConfiguration(string host, int port, ProtocolType protocol = ProtocolType.Tcp)
    {
        if (port is < 1024 or > 65535)
        {
            throw new ArgumentOutOfRangeException(nameof(Port), "Port must be between 1024 and 65535.");
        }

        if (string.IsNullOrWhiteSpace(host))
        {
            throw new ArgumentNullException(nameof(Host), "Host cannot be null or empty.");
        }

        if (protocol is ProtocolType.InProc && Port is not 0)
        {
            throw new ArgumentException("InProc protocol must have port as 0.");
        }


        Host = host;
        Port = port;
        Protocol = protocol;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConnectionConfiguration"/> class with default values.
    /// </summary>
    public ConnectionConfiguration()
    {
        Host = ConfigurationDefaults.DefaultHost;
        Port = ConfigurationDefaults.DefaultPort;
        Protocol = ProtocolType.Tcp;
    }
}