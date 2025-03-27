using Microsoft.Extensions.DependencyInjection;
using ZeroRPC.Server;
using ZeroRPC.NET.Common.Extensions;
using ZeroRPC.NET.Common.Types.Configuration;
using ZeroRPC.NET.Common.Constants;

var services = new ServiceCollection();

services.AddSingleton<IExampleService, ExampleService>();

// Initialize the ZeroRPC Router
services.AddZeroRpcServer();

var serviceProvider = services.BuildServiceProvider();

// Register the ZeroRPC services
serviceProvider.RegisterZeroRpcServices(new ConnectionConfiguration()
{
    Host = "*",
    Port = 5556,
    Protocol = ProtocolType.Tcp
}, new CancellationTokenSource().Token);