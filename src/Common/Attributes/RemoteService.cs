namespace ZeroRPC.NET.Common.Attributes;

/// <summary>
/// Represents an attribute that specifies the name and namespace of a remote service.
/// </summary>
[AttributeUsage(AttributeTargets.Interface)]
public class RemoteService : Attribute
{
    /// <summary>
    /// The name of the remote service.
    /// </summary>
    public string Name { get; }
    /// <summary>
    /// 
    /// </summary>
    public string Namespace { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RemoteService"/> class.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="namespaceName"></param>
    public RemoteService(string name, string namespaceName)
    {
        Name = name;
        Namespace = namespaceName;
    }
}