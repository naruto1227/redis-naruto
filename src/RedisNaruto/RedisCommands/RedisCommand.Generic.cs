using System.Net;
using RedisNaruto.Consts;
using RedisNaruto.Enums;
using RedisNaruto.Internal;
using RedisNaruto.Internal.Models;
using RedisNaruto.Models;
using RedisNaruto.Utils;

namespace RedisNaruto.RedisCommands;

/// <summary>
/// 基础命令
/// </summary>
public partial class RedisCommand : IRedisCommand
{
    /// <summary>
    /// 拷贝key
    /// </summary>
    /// <param name="source">原始key</param>
    /// <param name="dest">目标key</param>
    /// <param name="db">需要转移的目标的db</param>
    /// <param name="isReplace">如果true 代表如果目标key存在就先删除key</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<bool> CopyAsync(string source, string dest, int? db = null, bool isReplace = false,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(dest);
        cancellationToken.ThrowIfCancellationRequested();
        await using var client = await GetRedisClient(cancellationToken);
        var argv = new List<object>()
        {
            source,
            dest
        };
        if (db != null)
        {
            argv.Add(StreamConst.Db);
            argv.Add(db.Value);
        }

        argv.IfAdd(isReplace, StreamConst.Replace);

        return await client.ExecuteAsync(new Command(RedisCommandName.Copy, argv.ToArray())) == 1;
    }

    /// <summary>
    /// 删除
    /// </summary>
    /// <param name="keys">需要删除的key</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<int> DelAsync(string[] keys,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(keys);
        cancellationToken.ThrowIfCancellationRequested();
        await using var client = await GetRedisClient(cancellationToken);
        return await client.ExecuteAsync(new Command(RedisCommandName.Del, keys));
    }

    /// <summary>
    ///  以 Redis 特定格式序列化存储在 key 中的值，并将其返回给用户。可以使用命令将返回值合成回 Redis 键RESTORE 。
    /// </summary>
    /// <param name="key"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<RedisValue> DumpAsync(string key,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(key);
        cancellationToken.ThrowIfCancellationRequested();
        await using var client = await GetRedisClient(cancellationToken);
        return await client.ExecuteAsync(new Command(RedisCommandName.Dump, new[] {key}));
    }

    /// <summary>
    ///  将dump序列化的值，反序列化到指定的key中
    /// </summary>
    /// <param name="key"></param>
    /// <param name="expire">过期时间</param>
    /// <param name="isReplace">存储的话 先删除后添加</param>
    /// <param name="cancellationToken"></param>
    /// <param name="serializedValue">序列化值</param>
    /// <returns></returns>
    public async Task<bool> ReStoreAsync(string key, object serializedValue, TimeSpan? expire = null,
        bool isReplace = false,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(key);
        ArgumentNullException.ThrowIfNull(serializedValue);
        cancellationToken.ThrowIfCancellationRequested();
        var argv = new List<object>()
        {
            key,
            expire?.TotalMilliseconds ?? 0,
            serializedValue
        };
        argv.IfAdd(isReplace, StreamConst.Replace);

        await using var client = await GetRedisClient(cancellationToken);
        return await client.ExecuteAsync(new Command(RedisCommandName.ReStore, argv.ToArray())) == "OK";
    }

    /// <summary>
    /// 校验是否存在
    /// </summary>
    /// <param name="keys"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<int> ExistsAsync(string[] keys,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(keys);
        cancellationToken.ThrowIfCancellationRequested();
        await using var client = await GetRedisClient(cancellationToken);
        return await client.ExecuteAsync(new Command(RedisCommandName.Exists, keys));
    }

    /// <summary>
    /// 设置密钥的过期时间
    /// </summary>
    /// <param name="key"></param>
    /// <param name="expire">过期时间</param>
    /// <param name="expireEnum"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<int> ExpireAsync(string key, TimeSpan expire, ExpireEnum expireEnum = ExpireEnum.No,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(key);
        cancellationToken.ThrowIfCancellationRequested();
        await using var client = await GetRedisClient(cancellationToken);
        var argv = new List<object>()
        {
            key,
            expire.TotalSeconds
        };
        argv.IfAdd(expireEnum != ExpireEnum.No, expireEnum);
        return await client.ExecuteAsync(new Command(RedisCommandName.Expire, argv.ToArray()));
    }

    /// <summary>
    /// 设置密钥的过期时间
    /// </summary>
    /// <param name="key"></param>
    /// <param name="unixTimeSeconds">过期时间戳</param>
    /// <param name="expireEnum"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<int> ExpireAtAsync(string key, long unixTimeSeconds, ExpireEnum expireEnum = ExpireEnum.No,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(key);
        cancellationToken.ThrowIfCancellationRequested();
        await using var client = await GetRedisClient(cancellationToken);
        var argv = new List<object>()
        {
            key,
            unixTimeSeconds
        };
        argv.IfAdd(expireEnum != ExpireEnum.No, expireEnum);
        return await client.ExecuteAsync(new Command(RedisCommandName.ExpireAt, argv.ToArray()));
    }

    /// <summary>
    ///  返回给定密钥将过期的绝对 Unix 时间戳（自 1970 年 1 月 1 日起），以秒为单位。
    /// </summary>
    /// <param name="key"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<long> ExpireTimeAsync(string key, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(key);
        cancellationToken.ThrowIfCancellationRequested();
        await using var client = await GetRedisClient(cancellationToken);
        var argv = new List<object>()
        {
            key
        };
        return await client.ExecuteAsync(new Command(RedisCommandName.ExpireTime, argv.ToArray()));
    }

    /// <summary>
    ///  返回所有匹配的键pattern。
    /// </summary>
    /// <param name="pattern"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<List<RedisValue>> KeysAsync(string pattern, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(pattern);
        cancellationToken.ThrowIfCancellationRequested();
        await using var client = await GetRedisClient(cancellationToken);
        var argv = new List<object>()
        {
            pattern
        };
        return await client.ExecuteMoreResultAsync(new Command(RedisCommandName.Keys, argv.ToArray()))
            .ToRedisValueListAsync();
    }

    /// <summary>
    ///  迁移 原子性操作
    /// </summary>
    /// <param name="keys">需要迁移的key</param>
    /// <param name="remoteHost">目标主机</param>
    /// <param name="todatabase">目标库database</param>
    /// <param name="timeout">超时时间</param>
    /// <param name="isCopy">迁移成功本地key不删除</param>
    /// <param name="userName">auth2模式需要填写</param>
    /// <param name="cancellationToken"></param>
    /// <param name="isReplace">是否替换目标库 如果key存在的话</param>
    /// <param name="authEnum">授权类型</param>
    /// <param name="password">密码</param>
    /// <returns></returns>
    public async Task<bool> MiGrateAsync(string[] keys, IPEndPoint remoteHost, int todatabase, TimeSpan timeout,
        bool isCopy = false, bool isReplace = false, AuthEnum? authEnum = null, string password = null,
        string userName = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(keys);
        cancellationToken.ThrowIfCancellationRequested();
        await using var client = await GetRedisClient(cancellationToken);
        var argv = new List<object>()
        {
            remoteHost.Address.ToString(),
            remoteHost.Port,
            keys.Length == 1 ? keys[0] : "",
            todatabase,
            timeout.TotalMilliseconds
        };
        argv.IfAdd(isCopy, "COPY");
        argv.IfAdd(isReplace, "REPLACE");
        if (authEnum != null)
        {
            argv.Add(authEnum.ToString());
            if (authEnum == AuthEnum.Auth2)
            {
                argv.Add(userName);
            }

            argv.Add(password);
        }

        if (keys.Length > 1)
        {
            argv.Add("KEYS");
            argv.AddRange(keys);
        }

        return await client.ExecuteAsync(new Command(RedisCommandName.MiGrate, argv.ToArray())) == "OK";
    }

    /// <summary>
    /// key从当前选择的数据库（参见 参考资料）移动SELECT到指定的目标数据库。当key它已经存在于目标数据库中，或者它不存在于源数据库中时，它什么也不做。MOVE因此可以用作锁定原语。
    /// </summary>
    /// <param name="key"></param>
    /// <param name="db"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<bool> MoveAsync(string key, int db,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(key);
        cancellationToken.ThrowIfCancellationRequested();
        await using var client = await GetRedisClient(cancellationToken);
        var argv = new List<object>()
        {
            key,
            db
        };
        return await client.ExecuteAsync(new Command(RedisCommandName.Move, argv.ToArray())) == 1;
    }

    /// <summary>
    /// 返回对象的编码信息
    /// </summary>
    /// <param name="key"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<string> ObjectEncodingAsync(string key,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(key);
        cancellationToken.ThrowIfCancellationRequested();
        await using var client = await GetRedisClient(cancellationToken);
        var argv = new List<object>()
        {
            "ENCODING",
            key,
        };
        return await client.ExecuteAsync(new Command(RedisCommandName.Object, argv.ToArray()));
    }

    /// <summary>
    /// 返回对象的访问频率
    /// </summary>
    /// <param name="key"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<int> ObjectFreqAsync(string key,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(key);
        cancellationToken.ThrowIfCancellationRequested();
        await using var client = await GetRedisClient(cancellationToken);
        var argv = new List<object>()
        {
            "FREQ",
            key,
        };
        return await client.ExecuteAsync(new Command(RedisCommandName.Object, argv.ToArray()));
    }

    /// <summary>
    /// 返回对象的空闲时间
    /// </summary>
    /// <param name="key"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<int> ObjectIdleTimeAsync(string key,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(key);
        cancellationToken.ThrowIfCancellationRequested();
        await using var client = await GetRedisClient(cancellationToken);
        var argv = new List<object>()
        {
            "IDLETIME",
            key,
        };
        return await client.ExecuteAsync(new Command(RedisCommandName.Object, argv.ToArray()));
    }

    /// <summary>
    /// 对象重新计数
    /// </summary>
    /// <param name="key"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<int> ObjectRefCountAsync(string key,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(key);
        cancellationToken.ThrowIfCancellationRequested();
        await using var client = await GetRedisClient(cancellationToken);
        var argv = new List<object>()
        {
            "REFCOUNT",
            key,
        };
        return await client.ExecuteAsync(new Command(RedisCommandName.Object, argv.ToArray()));
    }

    /// <summary>
    /// 设置key的过期时间为永久生效
    /// </summary>
    /// <param name="key"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<bool> PersistAsync(string key,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(key);
        cancellationToken.ThrowIfCancellationRequested();
        await using var client = await GetRedisClient(cancellationToken);
        var argv = new List<object>()
        {
            key,
        };
        return await client.ExecuteAsync(new Command(RedisCommandName.Persist, argv.ToArray())) == 1;
    }


    /// <summary>
    /// 设置密钥的过期时间
    /// </summary>
    /// <param name="key"></param>
    /// <param name="expire">过期时间</param>
    /// <param name="expireEnum"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<int> PExpireAsync(string key, TimeSpan expire, ExpireEnum expireEnum = ExpireEnum.No,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(key);
        cancellationToken.ThrowIfCancellationRequested();
        await using var client = await GetRedisClient(cancellationToken);
        var argv = new List<object>()
        {
            key,
            expire.TotalMilliseconds
        };
        argv.IfAdd(expireEnum != ExpireEnum.No, expireEnum);
        return await client.ExecuteAsync(new Command(RedisCommandName.PExpire, argv.ToArray()));
    }

    /// <summary>
    /// 设置密钥的过期时间
    /// </summary>
    /// <param name="key"></param>
    /// <param name="unixTimeMillSeconds">过期时间戳 毫秒</param>
    /// <param name="expireEnum"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<int> PExpireAtAsync(string key, long unixTimeMillSeconds, ExpireEnum expireEnum = ExpireEnum.No,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(key);
        cancellationToken.ThrowIfCancellationRequested();
        await using var client = await GetRedisClient(cancellationToken);
        var argv = new List<object>()
        {
            key,
            unixTimeMillSeconds
        };
        argv.IfAdd(expireEnum != ExpireEnum.No, expireEnum);
        return await client.ExecuteAsync(new Command(RedisCommandName.PExpireAt, argv.ToArray()));
    }

    /// <summary>
    ///  返回给定密钥将过期的绝对 Unix 时间戳（自 1970 年 1 月 1 日起），以毫秒为单位。
    /// </summary>
    /// <param name="key"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<long> PExpireTimeAsync(string key, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(key);
        cancellationToken.ThrowIfCancellationRequested();
        await using var client = await GetRedisClient(cancellationToken);
        var argv = new List<object>()
        {
            key
        };
        return await client.ExecuteAsync(new Command(RedisCommandName.PExpireTime, argv.ToArray()));
    }

    /// <summary>
    ///  返回毫秒 过期时间
    /// </summary>
    /// <param name="key"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<long> PTtlAsync(string key, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(key);
        cancellationToken.ThrowIfCancellationRequested();
        await using var client = await GetRedisClient(cancellationToken);
        var argv = new List<object>()
        {
            key
        };
        return await client.ExecuteAsync(new Command(RedisCommandName.PTtl, argv.ToArray()));
    }

    /// <summary>
    ///  返回秒 过期时间
    /// </summary>
    /// <param name="key"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<long> TtlAsync(string key, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(key);
        cancellationToken.ThrowIfCancellationRequested();
        await using var client = await GetRedisClient(cancellationToken);
        var argv = new List<object>()
        {
            key
        };
        return await client.ExecuteAsync(new Command(RedisCommandName.Ttl, argv.ToArray()));
    }

    /// <summary>
    ///从当前选定的数据库中返回一个随机密钥
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<string> RandomKeyAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await using var client = await GetRedisClient(cancellationToken);
        return await client.ExecuteAsync(new Command(RedisCommandName.RandomKey, default));
    }

    /// <summary>
    /// 改名
    /// </summary>
    /// <param name="key"></param>
    /// <param name="newName">新名称</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<bool> ReNameAsync(string key, string newName, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(key);
        ArgumentNullException.ThrowIfNull(newName);
        cancellationToken.ThrowIfCancellationRequested();
        await using var client = await GetRedisClient(cancellationToken);
        var argv = new List<object>()
        {
            key,
            newName
        };
        return await client.ExecuteAsync(new Command(RedisCommandName.ReName, argv.ToArray())) == 1;
    }

    /// <summary>
    /// 改名
    /// </summary>
    /// <param name="key"></param>
    /// <param name="newName">新名称</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<bool> ReNameNxAsync(string key, string newName, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(key);
        ArgumentNullException.ThrowIfNull(newName);
        cancellationToken.ThrowIfCancellationRequested();
        await using var client = await GetRedisClient(cancellationToken);
        var argv = new List<object>()
        {
            key,
            newName
        };
        return await client.ExecuteAsync(new Command(RedisCommandName.ReNameNx, argv.ToArray())) == 1;
    }

    /// <summary>
    /// 获取数据类型
    /// </summary>
    /// <param name="key"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<string> TypeAsync(string key, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(key);
        cancellationToken.ThrowIfCancellationRequested();
        await using var client = await GetRedisClient(cancellationToken);
        var argv = new List<object>()
        {
            key,
        };
        return await client.ExecuteAsync(new Command(RedisCommandName.Type, argv.ToArray()));
    }

    /// <summary>
    /// 此命令与 非常相似DEL：它删除指定的键。就像DEL一个键如果不存在就会被忽略。但是，该命令在不同的线程中执行实际的内存回收，因此它不会阻塞，而DEL会。这就是命令名称的来源：该命令只是从键空间中取消链接键。实际删除将在稍后异步发生
    /// </summary>
    /// <param name="keys"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<int> UnLinkAsync(string[] keys, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(keys);
        cancellationToken.ThrowIfCancellationRequested();
        await using var client = await GetRedisClient(cancellationToken);
        return await client.ExecuteAsync(new Command(RedisCommandName.UnLink, keys));
    }

    /// <summary>
    /// 此命令会阻塞当前客户端，直到所有先前的写命令都成功传输并至少被指定数量的副本确认。如果达到以毫秒为单位指定的超时，即使尚未达到指定的副本数，命令也会返回。
    /// <see cref="https://redis.io/commands/wait/"/>
    /// </summary>
    /// <param name="numreplicas">副本数量</param>
    /// <param name="timeout">超时时间</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<int> WaitAsync(int numreplicas, TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await using var client = await GetRedisClient(cancellationToken);
        return await client.ExecuteAsync(new Command(RedisCommandName.Wait, new object[]
        {
            numreplicas,
            timeout?.TotalMilliseconds ?? 0
        }));
    }

    /// <summary>
    ///  此命令会阻塞当前客户端，直到所有先前的写入命令都被确认为已同步到本地 Redis 的 AOF 和/或至少指定数量的副本。如果达到以毫秒为单位指定的超时时间，即使未达到指定的确认次数，命令也会返回。
    /// <see cref="https://redis.io/commands/waitaof/"/>
    /// </summary>
    /// <param name="numlocal"></param>
    /// <param name="numreplicas">副本数量</param>
    /// <param name="timeout">超时时间</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<int> WaitAofAsync(int numlocal, int numreplicas, TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await using var client = await GetRedisClient(cancellationToken);
        return await client.ExecuteAsync(new Command(RedisCommandName.WaitAof, new object[]
        {
            numlocal,
            numreplicas,
            timeout?.TotalMilliseconds ?? 0
        }));
    }
    /// <summary>
    ///  更改密钥的最后访问时间。如果键不存在，则忽略该键。
    /// </summary>
    /// <param name="keys"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<int> TouchAsync(string[] keys, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(keys);
        cancellationToken.ThrowIfCancellationRequested();
        await using var client = await GetRedisClient(cancellationToken);
        return await client.ExecuteAsync(new Command(RedisCommandName.Touch, keys));
    }

}