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
internal static class SentinelConnection
{
    /// <summary>
    /// 消息传输
    /// </summary>
    private static readonly IMessageTransport MessageTransport = new MessageTransport();

    static SentinelConnection()
    {
        //初始化
    }

    /// <summary>
    /// 获取主节点地址
    /// </summary>
    /// <param name="masterName"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="RedisSentinelException"></exception>
    public static async Task<HostPort> GetMaserAddressAsync(string masterName,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var hostInfo = ConnectionStateManage.Get();
        //获取ip地址
        var ips = await Dns.GetHostAddressesAsync(hostInfo.hostPort.Host, cancellationToken);
        using var sentinelTcpClient = new TcpClient();
        await sentinelTcpClient.ConnectAsync(ips, hostInfo.hostPort.Port,
            cancellationToken);
        //执行获取主节点地址
        await MessageTransport.SendAsync(sentinelTcpClient.GetStream(), new object[]
        {
            RedisCommandName.Sentinel,
            "get-master-addr-by-name",
            // "sentinels",
            // "master",
            masterName
        });
        var result = await MessageTransport.ReciveAsync(sentinelTcpClient.GetStream());
        if (result is List<object> list)
        {
            return new HostPort(list[0].ToString(), list[1].ToString().ToInt());
        }

        throw new RedisSentinelException("获取主节点异常");
    }
}