namespace ZeroRPC.Advance.Server
{
    public class ExampleService : IExampleService
    {
        public int WaitAndReturn(int seconds)
        {
            Thread.Sleep(seconds * 1000);
            return seconds;
        }

        public async Task JustWaitAsync(int seconds)
        {
            Console.WriteLine("Waiting for {0} seconds", seconds);
            await Task.Delay(seconds * 1000);
        }

        public string ConcatString(string arg, string arg2)
        {
            return arg + arg2;
        }
    }
}