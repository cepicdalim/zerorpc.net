public record RpcRequest
{
    public string CorrelationId { get; } = "C-" + Guid.NewGuid().ToString();
    public string FullPath { get; }
    public string SerializedArgs { get; }
    public Type? ReturnType { get; }
    public TimeSpan Timeout { get; set; }
    public int RetryCount { get; set; }

    public RpcRequest(string fullPath, string serializedArgs, Type? returnType, int retryCount, TimeSpan timeout)
    {
        FullPath = fullPath;
        SerializedArgs = serializedArgs;
        ReturnType = returnType;
        RetryCount = retryCount;
        Timeout = timeout;
    }

    public int DecreaseRetryCount()
    {
        return RetryCount--;
    }
}