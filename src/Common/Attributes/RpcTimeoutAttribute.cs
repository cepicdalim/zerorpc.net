[AttributeUsage(AttributeTargets.Method)]
public class RemoteExecutionRule : Attribute
{
    public TimeSpan Timeout { get; }
    public int RetryCount { get; internal set; }
    /// <summary>
    /// Initializes a new instance of the <see cref="RemoteExecutionRule"/> class.
    /// <param name="timeoutMillisecond">The timeout in milliseconds.</param>
    /// <param name="retryCount">The number of times to retry the RPC call.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the timeout is greater than or equal to <see cref="int.MaxValue"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the timeout is less than or equal to zero.</exception>
    /// </summary>
    /// <param name="timeout">The timeout in milliseconds.</param>
    public RemoteExecutionRule(int timeoutMillisecond, int retryCount = 0)
    {
        if (timeoutMillisecond <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(timeoutMillisecond), "Timeout must be greater than zero.");
        }
        if (timeoutMillisecond >= int.MaxValue)
        {
            throw new ArgumentOutOfRangeException(nameof(timeoutMillisecond), "Timeout must be less than int.MaxValue.");
        }
        Timeout = TimeSpan.FromMilliseconds(timeoutMillisecond);

        if (retryCount < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(retryCount), "Retry count must be greater than or equal to zero.");
        }
        RetryCount = retryCount;
    }

    public int DecreaseRetryCount()
    {
        return RetryCount--;
    }
}