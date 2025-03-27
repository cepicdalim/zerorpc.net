# ZeroRPC for .NET

ZeroRPC is a .NET library designed to enable seamless communication between multiple applications using the ZeroMQ Dealer-Router pattern. It provides an elegant, attribute-based approach to defining remote methods and namespaces, making it easy to invoke server-side methods directly from client applications via interfaces.

This library offering lightweight, fast, and customizable Remote Procedure Call (RPC) functionality. With ZeroRPC, developers can focus on building distributed systems without worrying about the complexities of communication protocols.

---

## Features

- **Attribute-Based RPC**: Use `[RemoteMethod]`, `[RemoteService]`, and `[RemoteExecutionRule]` attributes to define remote methods and namespaces.
- **ZeroMQ Dealer-Router Pattern**: Leverages ZeroMQ for high-performance, asynchronous messaging.
- **Timeout Rules**: Configure timeout and retry logic for remote method calls.
- **Seamless Integration**: Easily integrate with .NET dependency injection for both clients and servers.
- **Task and Void Support**: Automatically handles return types, including `void` and `Task`.

---

## Installation

Install the library via NuGet:

```bash
dotnet add package ZeroRPC
```

---

## Getting Started

### 1. Define Remote Interfaces and Methods

Use the `[RemoteService]` and `[RemoteMethod]` attributes to define your remote interfaces and methods.

```csharp
[RemoteService("IMyService","MyApp")]
public interface IMyService
{
    [RemoteMethod("GetData")]
    [RemoteExecutionRule(timeoutMillisecond: 5000)]
    string GetData(int id);

    [RemoteMethod("SendMessage")]
    void SendMessage(string message);
}
```

---

### 2. Implement Server-Side Logic

Implement the interface on the server side.

```csharp
public class MyService : IMyService
{
    public string GetData(int id)
    {
        return $"Data for ID: {id}";
    }

    public void SendMessage(string message)
    {
        Console.WriteLine($"Message received: {message}");
    }
}
```

---

### 3. Setup the Server

Register the server and bind it to a port.

```csharp
var services = new ServiceCollection();

services.AddSingleton<IExampleService, ExampleService>();

// Register ZeroRPC server to DI
services.AddZeroRpcServer();

var serviceProvider = services.BuildServiceProvider();


var server = serviceProvider.GetRequiredService<ZeroRpcServer>();
var cancellationTokenSource = new CancellationTokenSource();
server.RunZeroRpcServer(new ConnectionConfiguration("*", 5556), cancellationTokenSource.Token);
Console.WriteLine("Server is running.");
Console.ReadLine();
```

---

### 4. Setup the Client

Register the client and connect to the server.

```csharp
var services = new ServiceCollection();

// Register example client
services.AddZeroRpcClient<IExampleService>(new ClientConfiguration()
{
    Connection = new ConnectionConfiguration("127.0.0.1", 5556, ProtocolType.Tcp),
    DefaultTimeout = TimeSpan.FromSeconds(15)
});

var serviceProvider = services.BuildServiceProvider();
var exampleService = serviceProvider.GetRequiredService<IExampleService>();

var simpleSync = exampleService.WaitAndReturn(0);
Console.WriteLine($"Simple sync: {simpleSync}");

var simpleAsync = await exampleService.WaitAndReturnAsync(0);
Console.WriteLine($"Simple async: {simpleAsync}");
```

---

## Advanced Features

### Timeout for Queries

Use `[RemoteExecutionRule]` to configure retries and timeouts for remote method calls.

```csharp
[RemoteExecutionRule(timeoutMillisecond: 10000)]
string GetData(int id);
```

---

### Exception Handling

ZeroRPC propagates server-side exceptions back to the client. You can handle exceptions as follows:

```csharp
try
{
    var data = client.GetData(999);
}
catch (ZeroRpcException ex)
{
    Console.WriteLine($"RPC Error: {ex.Message}");
}
```

---

## Example Code

### Server

```csharp
var services = new ServiceCollection();
services.AddZeroRpcServer();
services.AddSingleton<IMyService, MyService>();

var serviceProvider = services.BuildServiceProvider();
serviceProvider.RegisterZeroRpcServices(port: 5555);
```

### Client

```csharp
var services = new ServiceCollection();
services.AddZeroRpcClient<IMyService>(port: 5555);

var serviceProvider = services.BuildServiceProvider();
var client = serviceProvider.GetRequiredService<IMyService>();

var data = client.GetData(42);
Console.WriteLine(data);

client.SendMessage("Hello, server!");
```

---

## Example Project

Run both ZeroRPC.Client and ZeroRPC.Server inside examples folder projects using:

```bash
dotnet run
```

## How It Works

ZeroRPC uses the ZeroMQ Dealer-Router pattern for communication:

1. **Client**: Sends requests to the server using a `DealerSocket`.
2. **Server**: Listens for incoming requests using a `RouterSocket`.
3. **Attributes**: Define namespaces, methods, and execution rules for remote calls.
4. **Serialization**: Arguments and responses are serialized using JSON.
5. **Fire & Forget**: Do not wait for response for the methods that has return type of "void" or "Task"
6. **Request TTL**: Server doesn't process incoming requests if the timeout reached (client not listening for respnose anymore)

---

## Benchmark (via BenchmarkDotNet v0.14.0)

**Apple M1 Pro, 1 CPU, 10 logical and 10 physical cores**

| **Method**              |  **Mean** | **Error** | **StdDev** | **Median** | **Gen0** | **Allocated** |
| ----------------------- | --------: | --------: | ---------: | ---------: | -------: | ------------: |
| WaitAndReturn           | 302.21 us |  5.758 us |  13.906 us |  296.31 us |   0.4883 |        3.1 KB |
| WaitAndReturnAsync      | 302.85 us |  6.002 us |  12.790 us |  297.97 us |   0.4883 |       3.86 KB |
| MultipleParameter       | 305.07 us |  5.885 us |  11.888 us |  303.22 us |   0.4883 |        3.2 KB |
| WaitAndReturnModelAsync | 300.77 us |  6.006 us |   8.614 us |  298.94 us |   0.4883 |          4 KB |
| FireAndForgetAsync      |  77.88 us |  1.421 us |   1.187 us |   77.59 us |   0.2441 |       2.21 KB |

Run time: 00:03:03 (183.33 sec), executed benchmarks: 5

## Contributing

Contributions are welcome! If you find a bug or want to add a feature, feel free to open an issue or submit a pull request.

---

## License

ZeroRPC is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

---

## Links

- [ZeroMQ Documentation](https://zeromq.org/)
- [NetMQ](https://github.com/zeromq/netmq)

---

## Example Repository

Check out the example repository for a full implementation of ZeroRPC:
[ZeroRPC Example](https://github.com/cepicdalim/zerorpc.net/tree/main/examples/)

---

## Roadmap

- Adding unit tests
- Encapsulation for MQ library for further alternatives.
- Add support for streaming RPC calls.
- Improve error handling and logging.
- Optimize serialization/deserialization performance.
- Add more examples and documentation.

---

Feel free to copy and adapt this README for your GitHub repository!
