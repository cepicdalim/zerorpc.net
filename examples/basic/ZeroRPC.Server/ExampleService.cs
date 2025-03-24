namespace ZeroRPC.Server
{
    public class ExampleService : IExampleService
    {
        public int WaitAndReturn(int seconds)
        {
            Thread.Sleep(seconds * 1000);
            return seconds;
        }

        public string ConcatString(string arg, string arg2)
        {
            return arg + arg2;
        }
    }
}