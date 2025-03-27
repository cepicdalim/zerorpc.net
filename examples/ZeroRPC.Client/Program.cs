using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using ZeroRPC.Client;
using ZeroRPC.NET.Common.Constants;
using ZeroRPC.NET.Common.Extensions;
using ZeroRPC.NET.Common.Types.Configuration;
using ZeroRPC.NET.Common.Types.Exceptions;

class Program
{
    static async Task Main()
    {

        var services = new ServiceCollection();

        services.AddZeroRpcClient<IExampleService>(new ClientConfiguration()
        {
            Connection = new ConnectionConfiguration("127.0.0.1", 5556, ProtocolType.Tcp),
            DefaultTimeout = TimeSpan.FromSeconds(5)
        });

        var serviceProvider = services.BuildServiceProvider();
        var remoteExampleService = serviceProvider.GetRequiredService<IExampleService>();

        while (true)
        {
            Console.Clear();
            Console.WriteLine("ZeroRPC Client - Interactive Menu");
            Console.WriteLine("=====================================");
            Console.WriteLine("1. Fire and Forget");
            Console.WriteLine("2. Simple Sync Call");
            Console.WriteLine("3. Simple Async Call");
            Console.WriteLine("4. DTO Async Call");
            Console.WriteLine("5. Multiple Parameter Call");
            Console.WriteLine("6. Benchmark Async Calls");
            Console.WriteLine("7. Benchmark Sync Calls");
            Console.WriteLine("8. Timeout Example");
            Console.Write("Select an option: ");

            var input = Console.ReadLine();
            Console.Clear();

            switch (input)
            {
                case "1":
                    Console.WriteLine("Executing Fire and Forget...");
                    _ = remoteExampleService.FireAndForgetAsync(1);
                    Console.WriteLine("[Done]");
                    break;
                case "2":
                    Console.WriteLine("Executing Simple Sync Call...");
                    Console.WriteLine($"Result: {remoteExampleService.WaitAndReturn(1)}");
                    break;
                case "3":
                    Console.WriteLine("Executing Simple Async Call...");
                    Console.WriteLine($"Result: {await remoteExampleService.WaitAndReturnAsync(1)}");
                    break;
                case "4":
                    Console.WriteLine("Executing DTO Async Call...");
                    Console.WriteLine($"Result: {await remoteExampleService.WaitAndReturnModelAsync(0)}");
                    break;
                case "5":
                    Console.WriteLine("Executing Multiple Parameter Call...");
                    Console.WriteLine($"Result: {remoteExampleService.MultipleParameter("Hello", "World")}");
                    break;
                case "6":
                    await BenchmarkAsync(remoteExampleService);
                    break;
                case "7":
                    BenchmarkSync(remoteExampleService);
                    break;
                case "8":
                    Console.WriteLine("Executing Timeout Example...");
                    try
                    {
                        _ = await remoteExampleService.WaitAndReturnAsync(3);
                    }
                    catch (ZeroRpcException ex)
                    {
                        Console.WriteLine($"Timeout Exception: {ex.Message}");
                    }
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }
    }

    static async Task BenchmarkAsync(IExampleService service)
    {
        Console.WriteLine("Benchmarking Async Calls...");
        var stopwatch = Stopwatch.StartNew();
        var tasks = new List<Task>();
        for (var i = 0; i < 3; i++)
        {
            tasks.Add(service.WaitAndReturnAsync(1));
        }
        await Task.WhenAll(tasks);
        stopwatch.Stop();
        Console.WriteLine($"Elapsed Time: {stopwatch.ElapsedMilliseconds} ms");
    }

    static void BenchmarkSync(IExampleService service)
    {
        Console.WriteLine("Benchmarking Sync Calls...");
        var stopwatch = Stopwatch.StartNew();
        for (int i = 0; i < 3; i++)
        {
            service.WaitAndReturn(1);
        }
        stopwatch.Stop();
        Console.WriteLine($"Elapsed Time: {stopwatch.ElapsedMilliseconds} ms");
    }
}