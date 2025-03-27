using System.Collections.Concurrent;
using System.Reflection;
using ZeroRPC.NET.Common.Types.Configuration;
using ZeroRPC.NET.Common.Types.Dto;
using ZeroRPC.NET.Core;
using ZeroRPC.UnitTest.Core.Types;

namespace ZeroRPC.UnitTest.Core;

public class ZeroRpcClientTests
{
    public ZeroRpcClientTests()
    {
        ZeroRpcClient<ITestClientService>.InitializeClient(new ClientConfiguration());
    }

    [Test]
    public void ZeroRpcClient_Should_RegisterClientConfiguration()
    {
        var type = typeof(ZeroRpcClient<ITestClientService>);
        var field = type.GetField("_configurations", BindingFlags.Static | BindingFlags.NonPublic);
        var fieldValue = field?.GetValue(null) as ConcurrentDictionary<string, ClientConfiguration>;
        Assert.Multiple(() =>
        {
            Assert.That(field, Is.Not.Null);
            Assert.That(fieldValue, Is.Not.Null);
        });
        Assert.That(fieldValue?[typeof(ITestClientService).FullName!], Is.Not.Null);
    }

    [Test]
    public void ZeroRpcClient_Should_RegisterRequests()
    {
        var type = typeof(ZeroRpcClient<ITestClientService>);
        var field = type.GetField("_requests", BindingFlags.Static | BindingFlags.NonPublic);
        var fieldValue = field?.GetValue(null) as ConcurrentDictionary<string, RpcRequest>;
        Assert.Multiple(() =>
        {
            Assert.That(field, Is.Not.Null);
            Assert.That(fieldValue, Is.Not.Null);
        });
    }
    
    
}