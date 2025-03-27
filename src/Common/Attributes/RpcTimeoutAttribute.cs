namespace ZeroRPC.NET.Common.Attributes;

/// <summary>
/// Represents an attribute that specifies the timeout for a remote
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class RemoteExecutionRule : Attribute
{
    /// <summary>
    /// The timeout for the remote execution.
    /// </summary>
    public TimeSpan Timeout { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RemoteExecutionRule"/> class.
    /// <param name="timeoutMillisecond">The timeout in milliseconds.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the timeout is greater than or equal to <see cref="int.MaxValue"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the timeout is less than or equal to zero.</exception>
    /// </summary>
    public RemoteExecutionRule(int timeoutMillisecond)
    {
        switch (timeoutMillisecond)
        {
            case <= 0:
                throw new ArgumentOutOfRangeException(nameof(timeoutMillisecond), "Timeout must be greater than zero.");
            case >= int.MaxValue:
                throw new ArgumentOutOfRangeException(nameof(timeoutMillisecond), "Timeout must be less than int.MaxValue.");
        }

        Timeout = TimeSpan.FromMilliseconds(timeoutMillisecond);
    }
}