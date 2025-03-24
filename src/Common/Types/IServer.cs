public interface IServer
{
    void RegisterServices(int port, CancellationToken cancellationToken);
}