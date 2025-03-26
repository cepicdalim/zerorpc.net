using ZeroRPC.Advance.Client;
using ZeroRPC.NET.Common.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddZeroRpcClient<IExampleService>(port: 5557, defaultTimeout: TimeSpan.FromSeconds(5));
builder.Services.AddZeroRpcClient<IAnotherExampleService>(port: 5557, defaultTimeout: TimeSpan.FromSeconds(5));

var app = builder.Build();

app.MapGet("/query", (IExampleService service) =>
{
    int data = service.WaitAndReturn(1);
    return data;
});

app.MapGet("/query/timeout", (IExampleService service) =>
{
    // Default timeout is 5 seconds
    // ZeroRpcException: Timout reached while waiting for response from 
    // ZeroRPC.Advance.Server.IExampleService.WaitAndReturn
    int data = service.WaitAndReturn(6);
    return data;
});

app.MapGet("/nomethod", (IExampleService service) =>
{
    // ZeroRpcException: Method 'ZeroRPC.Advance.Server.IExampleService.NoMethod' not found.
    var data = service.NoMethod();
    return data;
});

app.MapGet("/command", async (IExampleService service) =>
{
    // Check the server logs to see the order of the commands are asyncronous
    await service.JustWaitAsync(3);
    await service.JustWaitAsync(3);
    await service.JustWaitAsync(3);
    return "It's not waiting at all!";
});

app.MapGet("/concat", (IAnotherExampleService service) =>
{
    string dataStr = service.ConcatList(["Hello", "World"]);
    return dataStr;
});



app.Run();