namespace ZeroRPC.NET.Common.Constants;

/// <summary>
/// Default configuration values.
/// </summary>
public static class ConfigurationDefaults
{
    /// <summary>
    /// The default host of the server.
    /// </summary>
    public const string DefaultHost = "localhost";

    /// <summary>
    /// The default port of the server.
    /// </summary>
    public const int DefaultPort = 5556;

    /// <summary>
    /// The default timeout for the client.
    /// </summary>
    public static TimeSpan DefaultTimeout = TimeSpan.FromSeconds(15);
}