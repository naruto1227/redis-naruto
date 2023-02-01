using System.Net;
using System.Net.Sockets;
using RedisNaruto.Exceptions;
using RedisNaruto.Internal.Message;
using RedisNaruto.Internal.Models;
using RedisNaruto.Utils;

namespace RedisNaruto.Internal.Sentinels;

/// <summary>
/// 哨兵
/// </summary>
internal sealed class SentinelConnection : ISentinelConnection
{
    /// <summary>
    /// 消息传输
    /// </summary>
    private readonly IMessageTransport _messageTransport = new MessageTransport();
    /// <summary>
    /// 哨兵客户端
    /// </summary>
    private TcpClient _sentinelTcpClient = new();

    /// <summary>
    /// 
    /// </summary>
    private readonly string _masterName;

    /// <summary>
    /// 哨兵
    /// </summary>
    /// <param name="masterName"></param>
    public SentinelConnection(string masterName)
    {
        _masterName = masterName;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="RedisSentinelException"></exception>
    public async Task<HostPort> GetMaserHostPort(
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var hostInfo = ConnectionStateManage.Get();
        //获取ip地址
        var ips = await Dns.GetHostAddressesAsync(hostInfo.hostPort.Host, cancellationToken);
        await _sentinelTcpClient.ConnectAsync(ips, hostInfo.hostPort.Port,
            cancellationToken);
        //执行获取主节点地址
        await _messageTransport.SendAsync(_sentinelTcpClient.GetStream(), new object[]
        {
            RedisCommandName.Sentinel,
            "get-master-addr-by-name",
            _masterName
        });
        var result = await _messageTransport.ReciveAsync(_sentinelTcpClient.GetStream());
        if (result is List<object> list)
        {
            return new HostPort(list[0].ToString(), list[1].ToString().ToInt());
        }

        throw new RedisSentinelException("获取主节点异常");
    }

    public void Dispose()
    {
        _sentinelTcpClient?.Close();
        _sentinelTcpClient = null;
        GC.SuppressFinalize(this);
    }
}