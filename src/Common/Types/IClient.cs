namespace ZeroRPC.NET.Common.Types;

/// <summary>
/// Represents a client that can invoke remote services.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IClient<T> where T : class;