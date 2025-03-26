using Microsoft.Extensions.DependencyInjection;
using ZeroRPC.Server;
using ZeroRPC.NET.Common.Extensions;

var services = new ServiceCollection();

services.AddSingleton<IExampleService, ExampleService>();

// Initialize the ZeroRPC Router
services.AddZeroRpcServer();

var serviceProvider = services.BuildServiceProvider();

// Register the ZeroRPC services
serviceProvider.RegisterZeroRpcServices(5556, new CancellationTokenSource().Token);
