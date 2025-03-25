namespace ZeroRPC.Advance.Server
{
    public class AnotherExampleService : IAnotherExampleService
    {
        public string ConcatList(List<string> list)
        {
            return string.Join(",", list);
        }

        public List<T> MergeList<T>(List<T> list1, List<T> list2)
        {
            list1.AddRange(list2);
            return list1;
        }
    }
}