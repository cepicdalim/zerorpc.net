using Microsoft.Extensions.DependencyInjection;
using ZeroRPC.Advance.Server;

var services = new ServiceCollection();

services.AddSingleton<IExampleService, ExampleService>();
services.AddSingleton<IAnotherExampleService, AnotherExampleService>();

// Initialize the ZeroRPC Router
services.AddZeroRpcServer();

var serviceProvider = services.BuildServiceProvider();

// Register the ZeroRPC services
serviceProvider.RegisterZeroRpcServices(5557, new CancellationTokenSource().Token);
