using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.IO;
using RedisNaruto.Exceptions;
using RedisNaruto.Internal.Interfaces;
using RedisNaruto.Internal.Message;
using RedisNaruto.Internal.Models;
using RedisNaruto.Internal.Serialization;
using RedisNaruto.Utils;

namespace RedisNaruto.Internal;

internal class RedisClient : IRedisClient
{
    /// <summary>
    /// 授权锁
    /// </summary>
    private readonly SemaphoreSlim _authLock = new(1);

    /// <summary>
    /// tcp 客户端
    /// </summary>
    protected TcpClient TcpClient { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public ConnectionModel ConnectionModel { get; }

    /// <summary>
    /// 当前连接的主机信息
    /// </summary>
    public string CurrentHost { get; }

    /// <summary>
    /// 当前连接的端口信息
    /// </summary>
    public int CurrentPort { get; }

    /// <summary>
    /// 是否授权
    /// </summary>
    protected bool IsAuth { get; set; }

    /// <summary>
    /// 序列化
    /// </summary>
    private readonly ISerializer _serializer = new Serializer();

    /// <summary>
    /// 消息传输
    /// </summary>
    private readonly IMessageTransport _messageTransport = new MessageTransport();

    private static readonly Random Random = new Random();

    /// <summary>
    /// 
    /// </summary>
    protected Func<IRedisClient, Task> DisposeTask;

    protected RedisClient()
    {
    }

    /// <summary>
    /// 
    /// </summary>
    public RedisClient(TcpClient tcpClient, ConnectionModel connectionModel, string currentHost, int currentPort,
        Func<IRedisClient, Task> disposeTask)
    {
        TcpClient = tcpClient;
        CurrentHost = currentHost;
        CurrentPort = currentPort;
        ConnectionModel = connectionModel;
        DisposeTask = disposeTask;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <param name="hostParameter"></param>
    /// <param name="connectionModel"></param>
    /// <returns></returns>
    internal static async Task<RedisClient> ConnectionAsync(ConnectionModel connectionModel,
        Func<IRedisClient, Task> disposeTask, CancellationToken cancellationToken = default,
        [CallerArgumentExpression("hosts")] string hostParameter = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var current = connectionModel.Connection[Random.Next(connectionModel.Connection.Length)];
        var hostString = current.Split(":");
        if (!int.TryParse(hostString[1], out var port))
        {
            port = 6349;
        }

        //初始化tcp客户端
        var tcpClient = new TcpClient();
        //获取ip地址
        var ips = await Dns.GetHostAddressesAsync(hostString[0], cancellationToken);
        await tcpClient.ConnectAsync(ips, port,
            cancellationToken);
        var currentHost = string.Join(',',
            ips.OrderBy(a => a.ToString()).Select(x => x.MapToIPv4().ToString()).ToArray());
        return new RedisClient(tcpClient, connectionModel, currentHost, port, disposeTask);
    }


    public async ValueTask DisposeAsync()
    {
        await this.DisposeCoreAsync(true);
    }

    protected virtual async ValueTask DisposeCoreAsync(bool isDispose)
    {
        if (isDispose)
        {
            await DisposeTask.Invoke(this);
        }
    }

    /// <summary>
    /// 关闭连接
    /// </summary>
    public void Close()
    {
        TcpClient?.Dispose();
        TcpClient = null;
        DisposeTask = null;
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// 执行命令
    /// </summary>
    /// <param name="command"></param>
    /// <typeparam name="TResult"></typeparam>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public virtual async Task<TResult> ExecuteAsync<TResult>(Command command)
    {
        var stream =
            await GetStreamAsync(command.Cmd is RedisCommandName.Auth or RedisCommandName.Quit);
        await _messageTransport.SendAsync(stream, command.CombinArgs());
        return await ReadMessageAsync<TResult>();
    }

    /// <summary>
    /// 读取消息
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public virtual async Task<TResult> ReadMessageAsync<TResult>()
    {
        var response = await _messageTransport.ReciveAsync(this.TcpClient.GetStream());
        if (response == default)
        {
            return default;
        }

        return response switch
        {
            TResult result => result,
            string obj => await _serializer.DeserializeAsync<TResult>(obj.ToEncode()),
            _ => throw new InvalidOperationException()
        };
    }

    /// <summary>
    /// 返回多结果集
    /// </summary>
    /// <param name="command"></param>
    /// <typeparam name="TResult"></typeparam>
    /// <returns></returns>
    public virtual async IAsyncEnumerable<TResult> ExecuteMoreResultAsync<TResult>(Command command)
    {
        var resultList = await ExecuteAsync<List<Object>>(command);
        var isStr = typeof(TResult) == typeof(string);
        foreach (var item in resultList)
        {
            if (isStr)
                yield return (TResult) item;
            else
                yield return await _serializer.DeserializeAsync<TResult>(item.ToEncode());
        }
    }

    /// <summary>
    /// 选择存储库
    /// </summary>
    /// <param name="db"></param>
    /// <returns></returns>
    public virtual async Task<bool> SelectDb(int db)
    {
        return (await ExecuteAsync<string>(new Command(RedisCommandName.Select, new Object[] {db}))) == "OK";
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public virtual async Task<bool> PingAsync()
    {
        return (await ExecuteAsync<string>(new Command(RedisCommandName.Ping, default))) == "PONG";
    }

    /// <summary>
    /// 授权
    /// </summary>
    private async Task AuthAsync()
    {
        if (!IsAuth)
        {
            using (await _authLock.LockAsync())
            {
                if (!IsAuth)
                {
                    IsAuth = await AuthAsync(ConnectionModel.UserName, ConnectionModel.Password);
                    await SelectDb(ConnectionModel.DataBase);
                }
            }
        }
    }

    /// <summary>
    /// 登陆
    /// </summary>
    /// <param name="userName"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public virtual async Task<bool> AuthAsync(string userName, string password)
    {
        if (userName.IsNullOrWhiteSpace())
            return (await ExecuteAsync<string>(new Command(RedisCommandName.Auth, new object[] {password}))) == "OK";
        if (userName.IsNullOrWhiteSpace() && !password.IsNullOrWhiteSpace())
            return (await ExecuteAsync<string>(new Command(RedisCommandName.Auth, new object[] {password}))) == "OK";
        return (await ExecuteAsync<string>(new Command(RedisCommandName.Auth, default))) == "OK";
    }

    /// <summary>
    /// 退出
    /// </summary>
    /// <returns></returns>
    public virtual async Task<bool> QuitAsync()
    {
        return (await ExecuteAsync<string>(new Command(RedisCommandName.Quit, default))) == "OK";
    }

    /// <summary>
    /// 重置
    /// </summary>
    /// <returns></returns>
    public virtual async Task ResetAsync(CancellationToken cancellationToken = default)
    {
        //检查连接是否有效
        if (await this.PingAsync())
        {
            return;
        }

        //切换新的连接 这里需要把此连接设置成无效状态 
        var hostInfo = ConnectionStateManage.Get();
        IsAuth = false;
        //释放连接
        TcpClient.Dispose();
        TcpClient = null;
        //重新打开一个新的连接
        var localClient = new TcpClient();
        await localClient.ConnectAsync(hostInfo.hostPort.Host, hostInfo.hostPort.Port, cancellationToken);
        TcpClient = localClient;
    }

    /// <summary>
    /// 获取流
    /// </summary>
    /// <returns></returns>
    private async Task<Stream> GetStreamAsync(bool isAuth)
    {
        if (!isAuth)
        {
            await AuthAsync();
        }

        return TcpClient.GetStream();
    }
}