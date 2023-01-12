using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.IO;
using RedisNaruto.Exceptions;
using RedisNaruto.Internal.Interfaces;
using RedisNaruto.Internal.Models;
using RedisNaruto.Internal.Serialization;
using RedisNaruto.Utils;

namespace RedisNaruto.Internal;

internal sealed class RedisClient : IRedisClient
{
    /// <summary>
    /// 池
    /// </summary>
    private static readonly RecyclableMemoryStreamManager MemoryStreamManager = new();

    /// <summary>
    /// 授权锁
    /// </summary>
    private readonly SemaphoreSlim _authLock = new(1);

    /// <summary>
    /// tcp 客户端
    /// </summary>
    private TcpClient _tcpClient;

    /// <summary>
    /// db访问库
    /// </summary>
    public int DB { get; }

    /// <summary>
    /// 用户名
    /// </summary>
    public string UserName { get; }

    /// <summary>
    /// 密码
    /// </summary>
    public string Password { get; }

    /// <summary>
    /// 是否授权
    /// </summary>
    private bool _isAuth;

    /// <summary>
    /// 换行
    /// </summary>
    private static readonly byte[] NewLine = "\r\n".ToEncode();

    /// <summary>
    /// 序列化
    /// </summary>
    private readonly ISerializer _serializer = new Serializer();

    private Func<IRedisClient, Task> _disposeTask;

    /// <summary>
    /// 
    /// </summary>
    internal RedisClient(TcpClient tcpClient, string userName, string password, int db,
        Func<IRedisClient, Task> disposeTask)
    {
        _tcpClient = tcpClient;
        UserName = userName;
        Password = password;
        DB = db;
        _disposeTask = disposeTask;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="hosts">主机信息</param>
    /// <param name="userName">用户名</param>
    /// <param name="password">密码</param>
    /// <param name="db"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="hostParameter"></param>
    /// <returns></returns>
    internal static async Task<RedisClient> ConnectionAsync(HostPort hostPort, string userName, string password, int db,
        Func<IRedisClient, Task> disposeTask, CancellationToken cancellationToken = default,
        [CallerArgumentExpression("hosts")] string hostParameter = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var tcpClient = new TcpClient();
        await tcpClient.ConnectAsync(await Dns.GetHostAddressesAsync(hostPort.Host, cancellationToken), hostPort.Port,
            cancellationToken);
        return new RedisClient(tcpClient, userName, password, db, disposeTask);
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
        await WriteArgAsync(stream, command.CombinArgs());
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
        var response = await GetResponseAsync(this._tcpClient.GetStream());
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
                    _isAuth = await AuthAsync(UserName, Password);
                    await SelectDb(DB);
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

    #region private

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

    /// <summary>
    /// 获取结果信息
    /// </summary>
    /// <param name="stream"></param>
    /// <returns></returns>
    private async Task<object> GetResponseAsync(Stream stream)
    {
        //获取首位的 符号 判断消息回复类型
        var bytes = new byte[1];
        _ = await stream.ReadAsync(bytes);
        var head = (char) bytes[0];
        switch (head)
        {
            case RespMessage.SimpleString:
            case RespMessage.Number:
            {
                var result = ReadLine(stream);
                return result;
            }
            //数组
            case RespMessage.ArrayString:
            {
                var result = await ReadMLineAsync(stream);
                return result;
            }
            case RespMessage.BulkStrings:
            {
                _ = ReadLine(stream);
                var result = ReadLine(stream);
                return result;
            }
            default:
            {
                //错误
                var result = ReadLine(stream);
                throw new RedisExecException(result);
            }
        }
    }

    /// <summary>
    /// 读取行数据
    /// </summary>
    /// <param name="stream"></param>
    /// <returns></returns>
    private string ReadLine(Stream stream)
    {
        var stringBuilder = new StringBuilder();
        while (true)
        {
            var msg = stream.ReadByte();
            if (msg < 0) break;
            //判断是否为换行 \r\n
            if (msg == '\r')
            {
                var msg2 = stream.ReadByte();
                if (msg2 < 0) break;
                if (msg2 == '\n') break;
                stringBuilder.Append((char) msg);
                stringBuilder.Append((char) msg2);
            }
            else
                stringBuilder.Append((char) msg);
        }

        return stringBuilder.ToString();
    }

    /// <summary>
    /// 多行读取
    /// </summary>
    /// <param name="stream"></param>
    /// <returns></returns>
    private async Task<List<Object>> ReadMLineAsync(Stream stream)
    {
        List<Object> resultList = new();

        //读取数组的长度
        var length = ReadLine(stream).ToInt();
        for (var i = 0; i < length; i++)
        {
            //获取 符号 判断消息类型 是字符串还是 数字 
            var bytes = new byte[1];
            _ = await stream.ReadAsync(bytes);
            var head = (char) bytes[0];
            switch (head)
            {
                case RespMessage.Number:
                {
                    var result = ReadLine(stream).ToLong();
                    resultList.Add(result);
                    break;
                }
                case RespMessage.BulkStrings:
                {
                    //去除第一位的长度
                    _ = ReadLine(stream);
                    //读取结果
                    var result = ReadLine(stream);
                    resultList.Add(result);
                    break;
                }
            }
        }

        return resultList;
    }

    /// <summary>
    /// 写入参数
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="args"></param>
    private async Task WriteArgAsync(Stream stream, object[] args)
    {
        if (args == null || args.Length <= 0)
        {
            return;
        }

        await using var ms = MemoryStreamManager.GetStream();
        ms.Position = 0;
        await ms.WriteAsync(await _serializer.SerializeAsync($"{RespMessage.ArrayString}{args.Length}"));
        await ms.WriteAsync(NewLine);
        //判断参数长度
        foreach (var item in args)
        {
            var argBytes = await _serializer.SerializeAsync(item);
            await ms.WriteAsync(await _serializer.SerializeAsync($"{RespMessage.BulkStrings}{argBytes.Length}"));
            await ms.WriteAsync(NewLine);
            await ms.WriteAsync(argBytes);
            await ms.WriteAsync(NewLine);
        }

        ms.Position = 0;
        await ms.CopyToAsync(stream);
    }

    #endregion
}