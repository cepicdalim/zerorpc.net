
using ZeroRPC.NET.Common.Attributes;

namespace ZeroRPC.Server
{
    public interface IExampleService
    {
        [RemoteMethod("WaitAndReturn")]
        int WaitAndReturn(int seconds);

        [RemoteMethod("ConcatString")]
        string ConcatString(string arg, string arg2);
    }
}