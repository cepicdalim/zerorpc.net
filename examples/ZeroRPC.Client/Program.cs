using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using ZeroRPC.Client;
using ZeroRPC.NET.Common.Extensions;
using ZeroRPC.NET.Common.Types.Configuration;

var services = new ServiceCollection();

// Register example client
services.AddZeroRpcClient<IExampleService>(new ClientConfiguration()
{
    Connection = new ConnectionConfiguration()
    {
        Host = "127.0.0.1",
        Port = 5556
    },
    DefaultTimeout = TimeSpan.FromSeconds(15)
});

var serviceProvider = services.BuildServiceProvider();
var remoteExampleService = serviceProvider.GetRequiredService<IExampleService>();

#region Basic Scenarios

_ = remoteExampleService.FireAndForgetAsync(3);

var simpleSync = remoteExampleService.WaitAndReturn(0);
Console.WriteLine($"Simple sync: {simpleSync}");

var simpleAsync = await remoteExampleService.WaitAndReturnAsync(0);
Console.WriteLine($"Simple async: {simpleAsync}");

var dtoAsync = await remoteExampleService.WaitAndReturnModelAsync(0);
Console.WriteLine($"Dto async: {dtoAsync}");

string dataStr = remoteExampleService.MultipleParameter("Hello", "World");
Console.WriteLine($"Multiple parameter: {dataStr}");
#endregion

#region Prof of async method
var stopwatch = new Stopwatch();
stopwatch.Start();
var tasks = new List<Task>();
for (int i = 0; i < 3; i++)
{
    tasks.Add(remoteExampleService.WaitAndReturnAsync(1));
}
await Task.WhenAll(tasks);
stopwatch.Stop();

// Should be approximately 1 seconds
Console.WriteLine($"Elapsed time: {stopwatch.ElapsedMilliseconds} ms");
#endregion

#region Prof of sync method
stopwatch.Restart();
for (int i = 0; i < 3; i++)
{
    remoteExampleService.WaitAndReturn(1);
}

stopwatch.Stop();
// Should be approximately 3 seconds
Console.WriteLine($"Elapsed time: {stopwatch.ElapsedMilliseconds} ms");
#endregion