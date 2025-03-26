
using ZeroRPC.NET.Common.Attributes;

namespace ZeroRPC.Client
{
    [RemoteService("IExampleService", "ZeroRPC.Server")]
    public interface IExampleService
    {
        int WaitAndReturn(int seconds);

        string ConcatString(string arg, string arg2);
    }
}