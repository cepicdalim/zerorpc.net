// benchmark.cs
using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.DependencyInjection;
using ZeroRPC.NET.Common.Extensions;

[assembly: BenchmarkCategory("ZeroRPC.NET")]

namespace ZeroRPC.Benchmark
{
    [MemoryDiagnoser]
    public class Benchmark
    {
        private readonly IExampleService _exampleService;

        public Benchmark()
        {
            var services = new ServiceCollection();
            services.AddSingleton<IExampleService, ExampleService>();
            services.AddZeroRpcClient<IExampleService>(port: 5556, defaultTimeout: TimeSpan.FromSeconds(15));
            var serviceProvider = services.BuildServiceProvider();
            _exampleService = serviceProvider.GetRequiredService<IExampleService>();
        }

        [Benchmark]
        public int WaitAndReturn()
        {
            return _exampleService.WaitAndReturn(0);
        }

        [Benchmark]
        public async Task<int> WaitAndReturnAsync()
        {
            return await _exampleService.WaitAndReturnAsync(0);
        }

        [Benchmark]
        public string MultipleParameter()
        {
            return _exampleService.MultipleParameter("Hello", "World");
        }

        [Benchmark]
        public async Task<ExampleDto> WaitAndReturnModelAsync()
        {
            return await _exampleService.WaitAndReturnModelAsync(0);
        }

        [Benchmark]
        public async Task FireAndForgetAsync()
        {
            await _exampleService.FireAndForgetAsync(0);
        }
    }
}