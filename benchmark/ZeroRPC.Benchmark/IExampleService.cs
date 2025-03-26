
using ZeroRPC.NET.Common.Attributes;

namespace ZeroRPC.Benchmark
{
    [RemoteService("IExampleService", "ZeroRPC.Server")]
    public interface IExampleService
    {
        Task FireAndForgetAsync(int seconds);
        int WaitAndReturn(int seconds);
        Task<ExampleDto> WaitAndReturnModelAsync(int seconds);
        Task<int> WaitAndReturnAsync(int seconds);
        string MultipleParameter(string arg, string arg2);
    }
}