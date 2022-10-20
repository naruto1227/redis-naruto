using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.IO;
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
    private readonly TcpClient _tcpClient;

    /// <summary>
    /// 用户名
    /// </summary>
    private string UserName { get; }

    /// <summary>
    /// 密码
    /// </summary>
    private string Password { get; }

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

    /// <summary>
    /// 
    /// </summary>
    private RedisClient(TcpClient tcpClient, string userName, string password)
    {
        _tcpClient = tcpClient;
        UserName = userName;
        Password = password;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="hosts">主机信息</param>
    /// <param name="userName">用户名</param>
    /// <param name="password">密码</param>
    /// <returns></returns>
    internal static async Task<RedisClient> ConnectionAsync(string[] hosts, string userName, string password,
        [CallerArgumentExpression("hosts")] string hostParameter = default)
    {
        if (hosts == null || hosts.Length <= 0)
        {
            throw new ArgumentNullException(hostParameter);
        }

        var hostString = hosts.FirstOrDefault().Split(":");
        if (!int.TryParse(hostString[1], out var port))
        {
            port = 6349;
        }

        var tcpClient = new TcpClient();
        await tcpClient.ConnectAsync(Dns.GetHostAddresses(hostString[0]), port);
        return new RedisClient(tcpClient, userName, password);
    }


    public ValueTask DisposeAsync()
    {
        _tcpClient.Dispose();
        return new ValueTask();
    }

    public async Task<TResult> ExecuteAsync<TResult>(Command command)
    {
        var stream =
            await GetRequestStreamAsync(command.Cmd == RedisCommandName.Auth || command.Cmd == RedisCommandName.Quit);
        await WriteArgAsync(stream, command.CombinArgs());
        var response = await GetResponseAsync(stream);
        switch (response)
        {
            case TResult result:
                return result;
            case string obj:
                return await _serializer.DeserializeAsync<TResult>(obj.ToEncode());
        }
        throw new InvalidOperationException();
    }

    /// <summary>
    /// 返回多结果集
    /// </summary>
    /// <param name="command"></param>
    /// <typeparam name="TResult"></typeparam>
    /// <returns></returns>
    public async Task<List<TResult>> ExecuteMoreResultAsync<TResult>(Command command)
    {
        var resultList = await ExecuteAsync<List<Object>>(command);
        List<TResult> list = new List<TResult>();
        var isStr = typeof(TResult) == typeof(string);
        foreach (var item in resultList)
        {
            if (isStr)
                list.Add((TResult) item);
            else
                list.Add(await _serializer.DeserializeAsync<TResult>(item.ToEncode()));
        }

        return list;
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
        await stream.ReadAsync(bytes);
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
                var result = ReadMLine(stream);
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
                throw new Exception(result);
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
    private List<Object> ReadMLine(Stream stream)
    {
        List<Object> resultList = new();

        //读取数组的长度
        var length = ReadLine(stream).ToInt();
        for (int i = 0; i < length; i++)
        {
            //第一位为数据长度 
            _ = ReadLine(stream);
            resultList.Add(ReadLine(stream));
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