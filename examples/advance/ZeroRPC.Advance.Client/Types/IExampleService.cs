
using ZeroRPC.NET.Common.Attributes;

namespace ZeroRPC.Advance.Client
{
    [RemoteService("IExampleService", "ZeroRPC.Advance.Server")]
    public interface IExampleService
    {
        int NoMethod();
        Task JustWaitAsync(int seconds);
        int WaitAndReturn(int seconds);
        string ConcatString(string arg, string arg2);
    }
}