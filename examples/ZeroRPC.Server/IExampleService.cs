
using ZeroRPC.NET.Common.Attributes;

namespace ZeroRPC.Server
{
    public interface IExampleService
    {
        [RemoteMethod("FireAndForgetAsync")]
        Task FireAndForgetAsync(int seconds);

        [RemoteMethod("WaitAndReturn")]
        int WaitAndReturn(int seconds);

        [RemoteMethod("WaitAndReturnAsync")]
        Task<int> WaitAndReturnAsync(int seconds);

        [RemoteMethod("WaitAndReturnModelAsync")]
        Task<ExampleDto> WaitAndReturnModelAsync(int seconds);

        [RemoteMethod("MultipleParameter")]
        string MultipleParameter(string arg, string arg2);
    }
}