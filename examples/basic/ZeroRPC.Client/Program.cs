using Microsoft.Extensions.DependencyInjection;
using ZeroRPC.Client;

var services = new ServiceCollection();

services.AddZeroRpcClient<IExampleService>(port: 5556, defaultTimeout: TimeSpan.FromSeconds(15));

var serviceProvider = services.BuildServiceProvider();

var remoteExampleService = serviceProvider.GetRequiredService<IExampleService>();

int data = remoteExampleService.WaitAndReturn(1);
Console.WriteLine(data);

string dataStr = remoteExampleService.ConcatString("Hello", "World");
Console.WriteLine(dataStr);


