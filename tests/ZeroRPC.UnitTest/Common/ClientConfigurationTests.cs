using ZeroRPC.NET.Common.Constants;
using ZeroRPC.NET.Common.Types.Configuration;

namespace ZeroRPC.UnitTest.Common;

public class Tests
{
    [Test]
    public void ClientConfiguration_Should_Be_Initialized()
    {
        var clientConfiguration = new ClientConfiguration()
        {
            DefaultTimeout = TimeSpan.FromSeconds(10),
            Connection = new ConnectionConfiguration()
        };
        Assert.That(clientConfiguration, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(clientConfiguration.Connection, Is.Not.Null);
            Assert.That(clientConfiguration.DefaultTimeout, Is.EqualTo(TimeSpan.FromSeconds(10)));
        });
    }

    [Test]
    public void ConnectionConfiguration_Should_Be_Initialized()
    {
        var connectionConfiguration = new ConnectionConfiguration("127.0.0.1", 4242, ProtocolType.InProc);
        Assert.That(connectionConfiguration, Is.Not.Null);
        Assert.That(connectionConfiguration.ToString(), Is.EqualTo("inproc://127.0.0.1:4242"));
    }

    [Test]
    public void ConnectionConfiguration_Should_Throw_Exception_When_Host_Is_Empty()
    {
        Assert.That(() => new ConnectionConfiguration("", 4242, ProtocolType.InProc), Throws.ArgumentNullException);
    }

    [Test]
    public void ConnectionConfiguration_Should_Throw_Exception_When_Port_Is_Invalid()
    {
        Assert.That(() => new ConnectionConfiguration("", 1000, ProtocolType.InProc), Throws.Exception);

        Assert.That(() => new ConnectionConfiguration("", 70000, ProtocolType.InProc), Throws.Exception);
    }
}