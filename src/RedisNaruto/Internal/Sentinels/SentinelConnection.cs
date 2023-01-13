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

    private static readonly Random Random = new Random();

    /// <summary>
    /// 哨兵客户端
    /// </summary>
    private static readonly TcpClient _sentinelTcpClient = new();

    /// <summary>
    /// 
    /// </summary>
    private readonly string[] _sentinelConnections;

    /// <summary>
    /// 
    /// </summary>
    private readonly string _masterName;

    /// <summary>
    /// 哨兵
    /// </summary>
    /// <param name="sentinelConnections"></param>
    /// <param name="masterName"></param>
    public SentinelConnection(string[] sentinelConnections, string masterName)
    {
        _sentinelConnections = sentinelConnections;
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
        //随机获取
        var current = _sentinelConnections[Random.Next(_sentinelConnections.Length)];
        var hostString = current.Split(":");
        if (!int.TryParse(hostString[1], out var port))
        {
            port = 6349;
        }

        //获取ip地址
        var ips = await Dns.GetHostAddressesAsync(hostString[0], cancellationToken);
        await _sentinelTcpClient.ConnectAsync(ips, port,
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
}