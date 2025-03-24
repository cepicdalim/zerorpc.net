[Serializable]
public class ZeroRpcException : Exception
{
    public ZeroRpcException() : base()
    {
    }


    public ZeroRpcException(Exception? ex)
    {
        _Message = ex?.Message ?? "Unknown error occurred.";
        _StackTrace = ex?.StackTrace;
    }

    public ZeroRpcException(string message)
    {
        _Message = message;
    }

    public override string StackTrace
    {
        get
        {
            return _StackTrace ?? "";
        }
    }

    public override string Message
    {
        get
        {
            return _Message ?? "Unknown error";
        }
    }

    public string? _StackTrace { get; set; }
    public string? _Message { get; set; }

}