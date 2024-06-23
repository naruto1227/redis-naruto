using System.Buffers;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Net.Sockets;
using RedisNaruto.EventDatas;
using RedisNaruto.Internal.DiagnosticListeners;
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
    /// 默认的db
    /// </summary>
    protected int DefaultDb { get; private set; }

    /// <summary>
    /// 标识 是否当前 连接是否开启了 客户端缓存指令
    /// </summary>
    public bool IsOpenTrackIng { get; private set; }

    /// <summary>
    /// 消息传输
    /// </summary>
    protected static readonly IMessageTransport MessageTransport = new MessageTransport();

    /// <summary>
    /// 
    /// </summary>
    protected Action<IRedisClient> DisposeTask;

    /// <summary>
    /// 
    /// </summary>
    public RedisClient(Guid connectionId, TcpClient tcpClient, ConnectionBuilder connectionBuilder, string currentHost,
        int currentPort,
        Action<IRedisClient> disposeTask)
    {
        TcpClient = tcpClient;
        CurrentHost = currentHost;
        CurrentPort = currentPort;
        ConnectionBuilder = connectionBuilder;
        DisposeTask = disposeTask;
        ConnectionId = connectionId;
        DefaultDb = connectionBuilder.DataBase;
        CurrentDb = connectionBuilder.DataBase;
    }

    public void Dispose()
    {
        this.DisposeCore(true);
    }

    /// <summary>
    /// 
    /// </summary>
    private bool _isDispose;

    // //todo 待校验
    // ~RedisClient()
    // {
    //     this.DisposeCore(false);
    // }

    protected void DisposeCore(bool isDispose)
    {
        if (isDispose)
        {
            DisposeTask.Invoke(this);
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
    /// 开启客户端缓存
    /// </summary>
    /// <returns></returns>
    public async Task UseClientSideCachingAsync()
    {
        if (!IsOpenTrackIng)
        {
            this.IsOpenTrackIng = true;
            //获取连接id
            await InitClientIdAsync();
            await BCastAsync(this.ClientId);
            //开启新线程 开启订阅模式
            new Thread(StartClientSideCaching).Start();
        }
    }

    /// <summary>
    /// 启用客户端缓存后台订阅服务
    /// </summary>
    private async void StartClientSideCaching()
    {
        //开启订阅
        //todo 记录返回的日志
        var res = await ExecuteAsync<object>(new Command(RedisCommandName.Sub, new object[] {"__redis__:invalidate"}));
        while (true)
        {
            try
            {
                //接收消息
                var message = await ReadMessageAsync();
                if (message is List<object> result && result.Count == 3)
                {
                    if (result[2] is List<object> cacheKeys)
                    {
                        foreach (var item in cacheKeys)
                        {
                            var cacheKey = (RedisValue) item;
                            //清除缓存key
                            ClientSideCachingDic.Remove(cacheKey);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                //todo 增加事件推送
                Console.WriteLine(e);
            }
        }
    }

    /// <summary>
    /// 初始化客户端id
    /// </summary>
    /// <returns></returns>
    public virtual async Task InitClientIdAsync()
    {
        //判断登录 这样如果是resp3 协议的话，后面就不需要多走一份代码
        await AuthAsync();
        if (string.IsNullOrWhiteSpace(ClientId) || ClientId.Length <= 0)
        {
            ClientId = await InvokeAsync(new Command(RedisCommandName.Client, new[]
            {
                "ID"
            }));
            Debug.WriteLine($"记录客户端id={ClientId}");
        }
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
            await GetStreamAsync(
                command.Cmd is RedisCommandName.Auth or RedisCommandName.Quit or RedisCommandName.Hello);
        await MessageSendAsync(stream, command);
        var result = await MessageTransport.ReceiveMessageAsync(stream);
        new WriteRedisNarutoMessageAfterEventData(command.Cmd, command.Args, result).WriteRedisNarutoAfter();
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
            await GetStreamAsync(
                command.Cmd is RedisCommandName.Auth or RedisCommandName.Quit or RedisCommandName.Hello);
        await MessageSendAsync(stream, command);
        var result = await MessageTransport.ReceiveSimpleMessageAsync(stream);
        new WriteRedisNarutoMessageAfterEventData(command.Cmd, command.Args, result).WriteRedisNarutoAfter();
        return result;
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
            await GetStreamAsync(
                command.Cmd is RedisCommandName.Auth or RedisCommandName.Quit or RedisCommandName.Hello);
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
            IsAuth = await AuthAsync(ConnectionBuilder.UserName, ConnectionBuilder.Password);
            if (!IsAuth)
            {
                throw new InvalidOperationException("login fail");
            }

            await SelectDb(ConnectionBuilder.DataBase);
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
        //  判断是否启用RESp3
        if (ConnectionBuilder.RESP3)
            return await SwitchRESP3(userName, password);
        if (userName.IsNullOrWhiteSpace() && !password.IsNullOrWhiteSpace())
            return (await InvokeAsync(new Command(RedisCommandName.Auth, new object[] {password}))) ==
                   "OK";
        if (!userName.IsNullOrWhiteSpace() && !password.IsNullOrWhiteSpace())
            return (await InvokeAsync(
                       new Command(RedisCommandName.Auth, new object[] {userName, password}))) ==
                   "OK";
        //没有密码的模式就不需要执行AUTH命令
        return true;
    }

    /// <summary>
    /// 切换到RESP3
    /// </summary>
    /// <param name="userName"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    private async Task<bool> SwitchRESP3(string userName, string password)
    {
        //无密码模式
        if (userName.IsNullOrWhiteSpace() && password.IsNullOrWhiteSpace())
        {
            return true;
        }

        Dictionary<string, object> redisValue = default;
        if (userName.IsNullOrWhiteSpace() && !password.IsNullOrWhiteSpace())
            redisValue = (await ExecuteAsync<Dictionary<string, object>>(new Command(RedisCommandName.Hello,
                new object[]
                {
                    3,
                    RedisCommandName.Auth,
                    password
                })));
        if (!userName.IsNullOrWhiteSpace() && !password.IsNullOrWhiteSpace())
            redisValue = (await ExecuteAsync<Dictionary<string, object>>(
                new Command(RedisCommandName.Hello, new object[]
                {
                    3,
                    RedisCommandName.Auth,
                    userName,
                    password
                })));
        //todo 记录服务器的信息
        var res = redisValue != default && redisValue.Count > 0;
        //resp3 协议中有返回clientid信息，这里直接获取赋值
        if (res && redisValue.TryGetValue("id", out var clientId))
        {
            ClientId = clientId.ToString();
        }

        return res;
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
        //清除消息
        await ClearMessageAsync(cancellationToken);
        //重置db
        await ResetDbAsync(cancellationToken);
    }

    /// <summary>
    /// 消息发送
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="command"></param>
    private async Task MessageSendAsync(Stream stream, Command command)
    {
        new WriteRedisNarutoMessageBeforeEventData(command.Cmd, command.Args).WriteRedisNarutoBefore();
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
        // await InitClientIdAsync();
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
    ///  重置db
    /// </summary>
    protected virtual async Task ResetDbAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (DefaultDb != CurrentDb)
        {
            await SelectDb(DefaultDb);
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

    #region 客户端缓存命令

    public virtual async Task BCastAsync(string clientId)
    {
        //判断是否开启了客户端缓存
        if (!this.ConnectionBuilder.IsOpenClientSideCaching)
        {
            return;
        }

        if (this.ConnectionBuilder.ClientSideCachingOption.KeyPrefix?.Length <= 0)
        {
            return;
        }

        //
        List<object> argv = new() //todo 临时写法
        {
            "TRACKING",
            "on",
            "REDIRECT",
            clientId,
            "BCAST",
        };

        foreach (var se in this.ConnectionBuilder!.ClientSideCachingOption!.KeyPrefix!)
        {
            argv.Add("PREFIX");
            argv.Add(se);
        }

        _ = await InvokeAsync(new Command(RedisCommandName.Client, argv.ToArray()));
    }

    #endregion
}