using System.Buffers;
using System.IO.Pipelines;
using System.Net.Sockets;
using RedisNaruto.Exceptions;
using RedisNaruto.Internal.Interfaces;
using RedisNaruto.Internal.Message;
using RedisNaruto.Internal.Models;
using RedisNaruto.Models;
using RedisNaruto.Utils;

namespace RedisNaruto.Internal.RedisClients;

internal class RedisClient : IRedisClient
{
    /// <summary>
    /// tcp 客户端
    /// </summary>
    protected TcpClient TcpClient { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public ConnectionBuilder ConnectionBuilder { get; }

    /// <summary>
    /// 连接id
    /// </summary>
    public Guid ConnectionId { get; private set; }

    /// <summary>
    /// 当前连接的主机信息
    /// </summary>
    public string CurrentHost { get; protected set; }

    /// <summary>
    /// 当前连接的端口信息
    /// </summary>
    public int CurrentPort { get; protected set; }

    /// <summary>
    /// 客户端id
    /// </summary>
    public string ClientId { get; protected set; }

    /// <summary>
    /// 是否授权
    /// </summary>
    protected bool IsAuth { get; set; }

    /// <summary>
    /// 最近一次包更新时间
    /// </summary>
    public long LastDataTime { get; private set; }

    /// <summary>
    /// 当前db
    /// </summary>
    public int CurrentDb { get; private set; }

    /// <summary>
    /// 消息传输
    /// </summary>
    protected static readonly IMessageTransport MessageTransport = new MessageTransport();

    /// <summary>
    /// 
    /// </summary>
    protected Func<IRedisClient, Task> DisposeTask;

    /// <summary>
    /// 
    /// </summary>
    public RedisClient(Guid connectionId, TcpClient tcpClient, ConnectionBuilder connectionBuilder, string currentHost,
        int currentPort,
        Func<IRedisClient, Task> disposeTask)
    {
        TcpClient = tcpClient;
        CurrentHost = currentHost;
        CurrentPort = currentPort;
        ConnectionBuilder = connectionBuilder;
        DisposeTask = disposeTask;
        ConnectionId = connectionId;
    }

    public async ValueTask DisposeAsync()
    {
        await this.DisposeCoreAsync(true);
    }

    /// <summary>
    /// todo 增加 是否释放的处理，为了在兮构函数判断
    /// </summary>
    private bool _isDispose;

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
    /// 初始化客户端id
    /// </summary>
    /// <returns></returns>
    public virtual async Task InitClientIdAsync()
    {
        //todo 待处理
        // ClientId = await InvokeAsync(new Command(RedisCommandName.Client, new[]
        // {
        //     "ID"
        // }));
    }

    /// <summary>
    /// 执行命令
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public virtual async Task<T> ExecuteAsync<T>(Command command)
    {
        var stream =
            await GetStreamAsync(command.Cmd is RedisCommandName.Auth or RedisCommandName.Quit);
        await MessageSendAsync(stream, command);
        var result = await MessageTransport.ReceiveMessageAsync(stream);
        if (result is T redisValue)
        {
            return redisValue;
        }

        return default(T);
    }

    /// <summary>
    /// 执行命令
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public virtual async Task<RedisValue> ExecuteSampleAsync(Command command)
    {
        var stream =
            await GetStreamAsync(command.Cmd is RedisCommandName.Auth or RedisCommandName.Quit);
        await MessageSendAsync(stream, command);
        return await MessageTransport.ReceiveSimpleMessageAsync(stream);
    }

    /// <summary>
    /// 执行命令 无返回值
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public virtual async Task ExecuteNoResultAsync(Command command)
    {
        var stream =
            await GetStreamAsync(command.Cmd is RedisCommandName.Auth or RedisCommandName.Quit);
        await MessageSendAsync(stream, command);
    }

    /// <summary>
    /// 读取消息
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<object> ReadMessageAsync()
    {
        return await MessageTransport.ReceiveMessageAsync(await GetStreamAsync(true));
    }

    /// <summary>
    /// 选择存储库
    /// </summary>
    /// <param name="db"></param>
    /// <returns></returns>
    public virtual async Task<bool> SelectDb(int db)
    {
        if (db == CurrentDb)
        {
            return true;
        }

        var res = (await InvokeAsync(new Command(RedisCommandName.Select, new object[] {db}))) == "OK";
        if (res)
        {
            CurrentDb = db;
        }

        return res;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public virtual async Task<bool> PingAsync()
    {
        return (await InvokeAsync(new Command(RedisCommandName.Ping, default))) == "PONG";
    }


    /// <summary>
    /// 执行命令
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    public async Task<RedisValue> InvokeAsync(Command command)
    {
        return await ExecuteSampleAsync(command);
    }

    /// <summary>
    /// 授权
    /// </summary>
    private async Task AuthAsync()
    {
        if (!IsAuth)
        {
            // using (await _authLock.LockAsync())
            // {
            //     if (!IsAuth)
            //     {
            IsAuth = await AuthAsync(ConnectionBuilder.UserName, ConnectionBuilder.Password);
            if (!IsAuth)
            {
                throw new InvalidOperationException("login fail");
            }

            await SelectDb(ConnectionBuilder.DataBase);
            // }
            // }
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
        if (userName.IsNullOrWhiteSpace() && !password.IsNullOrWhiteSpace())
            return (await InvokeAsync(new Command(RedisCommandName.Auth, new object[] {password}))) ==
                   "OK";
        if (!userName.IsNullOrWhiteSpace() && !password.IsNullOrWhiteSpace())
            return (await InvokeAsync(
                       new Command(RedisCommandName.Auth, new object[] {userName, password}))) ==
                   "OK";
        return (await InvokeAsync(new Command(RedisCommandName.Auth, default))) == "OK";
    }

    /// <summary>
    /// 退出
    /// </summary>
    /// <returns></returns>
    public virtual async Task<bool> QuitAsync()
    {
        return (await InvokeAsync(new Command(RedisCommandName.Quit, default))) == "OK";
    }

    /// <summary>
    /// 重置
    /// </summary>
    /// <returns></returns>
    public virtual async Task ResetAsync(CancellationToken cancellationToken = default)
    {
        await ClearMessageAsync(cancellationToken);
    }

    /// <summary>
    /// 消息发送
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="command"></param>
    private async Task MessageSendAsync(Stream stream, Command command)
    {
        await MessageTransport.SendAsync(stream, command);
        //更新时间
        this.LastDataTime = TimeUtil.GetTimestamp();
    }

    /// <summary>
    /// 重置socket 
    /// </summary>
    /// <param name="cancellationToken"></param>
    public virtual async Task ResetSocketAsync(CancellationToken cancellationToken = default)
    {
        //设置连接状态无效
        ConnectionStateManage.SetInValid(ConnectionId);
        //切换新的连接
        var hostInfo = ConnectionStateManage.Get();
        ConnectionId = hostInfo.connectionId;
        IsAuth = false;
        //释放连接
        TcpClient?.Dispose();
        TcpClient = null;
        //重新打开一个新的连接
        var localClient = new TcpClient()
        {
            ReceiveTimeout = ConnectionBuilder.TimeOut,
            SendTimeout = ConnectionBuilder.TimeOut
        };
        await localClient.ConnectAsync(hostInfo.hostPort.Host, hostInfo.hostPort.Port, cancellationToken);
        TcpClient = localClient;
        CurrentHost = hostInfo.hostPort.Host;
        CurrentPort = hostInfo.hostPort.Port;
        CurrentDb = ConnectionBuilder.DataBase;
        //登陆
        await AuthAsync();
        await InitClientIdAsync();
    }

    /// <summary>
    /// 清空所有的消息
    /// </summary>
    /// <param name="cancellationToken"></param>
    protected virtual async Task ClearMessageAsync(CancellationToken cancellationToken = default)
    {
        var tcpClient = TcpClient;
        if (tcpClient.Available <= 0)
        {
            return;
        }

        var stream = await GetStreamAsync(true);
        while (tcpClient.Available > 0)
        {
            using (var memoryOwner = MemoryPool<byte>.Shared.Rent(1024))
            {
                _ = await stream.ReadAsync(memoryOwner.Memory, cancellationToken);
            }
        }
    }

    /// <summary>
    /// 获取流
    /// </summary>
    /// <param name="isSkipAuth">是否需要跳过授权</param>
    /// <returns></returns>
    private async Task<Stream> GetStreamAsync(bool isSkipAuth)
    {
        if (!isSkipAuth)
        {
            await AuthAsync();
        }

        var tcp = TcpClient;
        if (tcp.Connected) return tcp.GetStream();
        await ResetSocketAsync();
        tcp = TcpClient;

        return tcp.GetStream();
    }
}