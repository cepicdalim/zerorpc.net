
using ZeroRPC.NET.Common.Attributes;

namespace ZeroRPC.Advance.Server
{
    public interface IAnotherExampleService
    {
        [RemoteMethod("ConcatList")]
        public string ConcatList(List<string> list);

        [RemoteMethod("MergeList")]
        public List<T> MergeList<T>(List<T> list1, List<T> list2);
    }
}