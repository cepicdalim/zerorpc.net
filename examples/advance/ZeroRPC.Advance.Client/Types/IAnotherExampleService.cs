
using ZeroRPC.NET.Common.Attributes;

namespace ZeroRPC.Advance.Client
{
    [RemoteService("IAnotherExampleService", "ZeroRPC.Advance.Server")]
    public interface IAnotherExampleService
    {
        public string ConcatList(List<string> list);
        public List<T> MergeList<T>(List<T> list1, List<T> list2);
    }
}