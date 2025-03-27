using ZeroRPC.NET.Common.Types.Configuration;

namespace ZeroRPC.NET.Common.Types;

/// <summary>
///  Represents a server that can expose services.
/// </summary>
public interface IServer
{
    /// <summary>
    /// Register services to be exposed by the server.
    /// </summary>
    /// <param name="connectionConfiguration"></param>
    /// <param name="cancellationToken"></param>
    void RegisterServices(ConnectionConfiguration connectionConfiguration, CancellationToken cancellationToken);
}