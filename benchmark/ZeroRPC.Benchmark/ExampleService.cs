namespace ZeroRPC.Benchmark
{
    public class ExampleService : IExampleService
    {
        public int WaitAndReturn(int seconds)
        {
            Thread.Sleep(seconds * 1000);
            return seconds;
        }

        public async Task<int> WaitAndReturnAsync(int seconds)
        {
            await Task.Delay(seconds * 1000);
            return seconds;
        }

        public string MultipleParameter(string arg, string arg2)
        {
            return arg + arg2;
        }

        public async Task<ExampleDto> WaitAndReturnModelAsync(int seconds)
        {
            await Task.Delay(seconds * 1000);
            return new ExampleDto { Name = "Hello" };
        }

        public async Task FireAndForgetAsync(int seconds)
        {
            await Task.Delay(seconds * 1000);
        }
    }
}