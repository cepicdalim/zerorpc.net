[AttributeUsage(AttributeTargets.Interface)]
public class RemoteService : Attribute
{
    public string Name { get; }
    public string Namespace { get; }

    public RemoteService(string name, string namespaceName)
    {
        Name = name;
        Namespace = namespaceName;
    }
}