namespace ZeroRPC.NET.Common.Attributes;

/// <summary>
/// Represents an attribute that specifies the name of a remote method.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class RemoteMethod : Attribute
{
    /// <summary>
    /// The name of the remote method.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RemoteMethod"/> class.
    /// </summary>
    /// <param name="name"></param>
    public RemoteMethod(string name)
    {
        Name = name;
    }
}