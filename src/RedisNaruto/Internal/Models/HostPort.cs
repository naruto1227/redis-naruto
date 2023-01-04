namespace RedisNaruto.Internal.Models;

public struct HostPort
{
    public HostPort(string host,int port)
    {
        this.Host = host;
        this.Port = port;
    }
    public string Host { get; init; }

    public int Port { get; init; }
}