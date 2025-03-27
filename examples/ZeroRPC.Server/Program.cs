using Microsoft.Extensions.DependencyInjection;
using ZeroRPC.Server;
using ZeroRPC.NET.Common.Extensions;
using ZeroRPC.NET.Common.Types.Configuration;
using ZeroRPC.NET.Core;

var services = new ServiceCollection();

services.AddSingleton<IExampleService, ExampleService>();

// Register ZeroRPC server to DI
services.AddZeroRpcServer();

var serviceProvider = services.BuildServiceProvider();


var server = serviceProvider.GetRequiredService<ZeroRpcServer>();
var cancellationTokenSource = new CancellationTokenSource();
server.RunZeroRpcServer(new ConnectionConfiguration("*", 5556), cancellationTokenSource.Token);

Console.WriteLine("Server is running. Press any key to stop ZeroRPC Server.");
Console.ReadLine();

cancellationTokenSource.Cancel();

Console.WriteLine("ZeroRPC Server stopped. Press any key to exit.");
Console.ReadLine();