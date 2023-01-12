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

internal sealed class RedisClient : IRedisClient
{
    /// <summary>
    /// 授权锁
    /// </summary>
    private readonly SemaphoreSlim _authLock = new(1);

    /// <summary>
    /// tcp 客户端
    /// </summary>
    private TcpClient _tcpClient;

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
    private bool _isAuth;

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
    private Func<IRedisClient, Task> _disposeTask;

    /// <summary>
    /// 
    /// </summary>
    private RedisClient(TcpClient tcpClient, ConnectionModel connectionModel,string currentHost,int currentPort,
        Func<IRedisClient, Task> disposeTask)
    {
        _tcpClient = tcpClient;
        CurrentHost = currentHost;
        CurrentPort = currentPort;
        ConnectionModel = connectionModel;
        _disposeTask = disposeTask;
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
        var current= connectionModel.Connection[Random.Next(connectionModel.Connection.Length)];
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
        var currentHost = string.Join(',', ips.OrderBy(a => a.ToString()).Select(x => x.MapToIPv4().ToString()).ToArray());
        return new RedisClient(tcpClient, connectionModel,currentHost,port, disposeTask);
    }


    public async ValueTask DisposeAsync()
    {
        await this.DisposeCoreAsync(true);
    }

    private async ValueTask DisposeCoreAsync(bool isDispose)
    {
        if (isDispose)
        {
            await _disposeTask.Invoke(this);
        }
    }

    /// <summary>
    /// 关闭连接
    /// </summary>
    public void Close()
    {
        _tcpClient?.Dispose();
        _tcpClient = null;
        _disposeTask = null;
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// 执行命令
    /// </summary>
    /// <param name="command"></param>
    /// <typeparam name="TResult"></typeparam>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<TResult> ExecuteAsync<TResult>(Command command)
    {
        var stream =
            await GetRequestStreamAsync(command.Cmd is RedisCommandName.Auth or RedisCommandName.Quit);
        await _messageTransport.SendAsync(stream, command.CombinArgs());
        return await ReadMessageAsync<TResult>();
    }

    /// <summary>
    /// 读取消息
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<TResult> ReadMessageAsync<TResult>()
    {
        var response = await _messageTransport.ReciveAsync(this._tcpClient.GetStream());
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
    public async IAsyncEnumerable<TResult> ExecuteMoreResultAsync<TResult>(Command command)
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
    public async Task<bool> SelectDb(int db)
    {
        return (await ExecuteAsync<string>(new Command(RedisCommandName.Select, new Object[] {db}))) == "OK";
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public async Task<bool> PingAsync()
    {
        return (await ExecuteAsync<string>(new Command(RedisCommandName.Ping, default))) == "PONG";
    }

    /// <summary>
    /// 授权
    /// </summary>
    private async Task AuthAsync()
    {
        if (!_isAuth)
        {
            using (await _authLock.LockAsync())
            {
                if (!_isAuth)
                {
                    _isAuth = await AuthAsync(ConnectionModel.UserName, ConnectionModel.Password);
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
    public async Task<bool> AuthAsync(string userName, string password)
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
    public async Task<bool> QuitAsync()
    {
        return (await ExecuteAsync<string>(new Command(RedisCommandName.Quit, default))) == "OK";
    }

    /// <summary>
    /// 获取流
    /// </summary>
    /// <returns></returns>
    private async Task<Stream> GetRequestStreamAsync(bool isAuth)
    {
        if (!isAuth)
        {
            await AuthAsync();
        }

        return _tcpClient.GetStream();
    }
}