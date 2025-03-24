[AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
public class RemoteMethod : Attribute
{
    public string Name { get; }

    public RemoteMethod(string name)
    {
        Name = name;
    }
}