using RedisNaruto.Internal;
using RedisNaruto.Internal.Models;
using RedisNaruto.Utils;

namespace RedisNaruto.RedisCommands;

/// <summary>
/// 字符串操作
/// </summary>
public partial class RedisCommand : IRedisCommand
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="timeSpan"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<bool> SetAsync(string key, object value, TimeSpan timeSpan = default,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var argv = timeSpan == default
            ? new[]
            {
                key,
                value
            }
            : new[]
            {
                key,
                value,
                "PX",
                timeSpan == default ? 0 : timeSpan.TotalMilliseconds
            };
        await using var client = await GetRedisClient(cancellationToken);

        var result =
            await client.ExecuteAsync<string>(new Command(RedisCommandName.Set, argv));
        return result == "OK";
    }

    /// <summary>
    /// 如果key不存在 才添加值
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<bool> SetNxAsync(string key, object value,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await using var client = await GetRedisClient(cancellationToken);
        var result =
            await client.ExecuteAsync<string>(new Command(RedisCommandName.SetNx, new[]
            {
                key, value
            }));
        return result == "1";
    }

    /// <summary>
    /// 返回字符串的长度
    /// </summary>
    /// <param name="key"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<long> StrLenAsync(string key,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await using var client = await GetRedisClient(cancellationToken);
        var result =
            await client.ExecuteAsync<long>(new Command(RedisCommandName.StrLen, new[]
            {
                key
            }));
        return result;
    }

    /// <summary>
    /// 覆盖字符串的值 从指定的偏移量开始
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="offset"></param>
    /// <returns></returns>
    public async Task<long> SetRangeAsync(string key,long offset,string value,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await using var client = await GetRedisClient(cancellationToken);
        var result =
            await client.ExecuteAsync<long>(new Command(RedisCommandName.SetRange, new object[]
            {
                key,offset,value
            }));
        return result;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="vals"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<bool> MSetAsync(Dictionary<string, string> vals,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var argv = new object[vals.Count * 2];

        var idx = 1;
        foreach (var (key, value) in vals)
        {
            argv[idx * 2 - 2] = key;
            argv[idx * 2 - 1] = value;
            idx++;
        }

        await using var client = await GetRedisClient(cancellationToken);

        var result =
            await client.ExecuteAsync<string>(new Command(RedisCommandName.MSet, argv));
        return result == "OK";
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<string> GetAsync(string key, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await GetAsync<string>(key, cancellationToken);
    }

    /// <summary>
    /// 查询字符串
    /// </summary>
    /// <param name="key"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<TResult> GetAsync<TResult>(string key, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await using var client = await GetRedisClient(cancellationToken);
        return await client.ExecuteAsync<TResult>(new Command(RedisCommandName.Get,
            new object[] {key}));
    }

    /// <summary>
    /// 批量获取
    /// </summary>
    /// <param name="key"></param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="TResult"></typeparam>
    /// <returns></returns>
    public async Task<List<TResult>> MGetAsync<TResult>(string[] key,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await using var client = await GetRedisClient(cancellationToken);
        return await (client.ExecuteMoreResultAsync<TResult>(
            new Command(RedisCommandName.MGET, key))).ToListAsync();
    }

    ///  <summary>
    /// 如果key已经存在并且是一个字符串，此命令将value在字符串末尾附加。如果key不存在，则创建它并将其设置为空字符串，因此与这种特殊情况APPEND 类似SET。
    ///  </summary>
    ///  <param name="key"></param>
    ///  <param name="val">值</param>
    ///  <param name="cancellationToken"></param>
    ///  <returns></returns>
    public async Task AppendAsync(string key, string val, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await using var client = await GetRedisClient(cancellationToken);
        _ = await client.ExecuteAsync<int>(new Command(RedisCommandName.Append, new object[] {key, val}));
    }

    /// <summary>
    /// 按照指定的值递减
    /// </summary>
    /// <param name="key"></param>
    /// <param name="val"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<long> DecrByAsync(string key, long val = 1, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await using var client = await GetRedisClient(cancellationToken);
        return await client.ExecuteAsync<long>(new Command(RedisCommandName.DecrBy, new object[] {key, val}));
    }

    /// <summary>
    /// 查询指定的键值 如果存在就删除
    /// </summary>
    /// <param name="key"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<string> GetDelAsync(string key, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await using var client = await GetRedisClient(cancellationToken);
        return await client.ExecuteAsync<string>(new Command(RedisCommandName.GetDel, new object[] {key}));
    }

    ///  <summary>
    /// 查询key的值 并设置过期时间
    ///  </summary>
    ///  <param name="key"></param>
    ///  <param name="expireTime">过期时间</param>
    ///  <param name="cancellationToken"></param>
    ///  <returns></returns>
    public async Task<string> GetExAsync(string key, TimeSpan expireTime,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await using var client = await GetRedisClient(cancellationToken);
        return await client.ExecuteAsync<string>(new Command(RedisCommandName.GetEx,
            new object[] {key, "PX", expireTime.TotalMilliseconds}));
    }

    ///  <summary>
    /// 获取字符串 指定区间的值
    ///  </summary>
    ///  <param name="key"></param>
    ///  <param name="end">字符串的结束下标</param>
    ///  <param name="cancellationToken"></param>
    ///  <param name="begin">字符串的开始下标</param>
    ///  <returns></returns>
    public async Task<string> GetRangeAsync(string key, int begin, int end,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await using var client = await GetRedisClient(cancellationToken);
        return await client.ExecuteAsync<string>(new Command(RedisCommandName.GetRange,
            new object[] {key, begin, end}));
    }

    /// <summary>
    /// 按照指定的值递增
    /// </summary>
    /// <param name="key"></param>
    /// <param name="val"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<long> IncrByAsync(string key, long val = 1, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await using var client = await GetRedisClient(cancellationToken);
        return await client.ExecuteAsync<long>(new Command(RedisCommandName.IncrBy, new object[] {key, val}));
    }

    /// <summary>
    /// 匹配两个字符串的相似程度 返回匹配成功的内容
    /// </summary>
    /// <param name="key1"></param>
    /// <param name="key2"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<string> LcsWithStringAsync(string key1, string key2,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await using var client = await GetRedisClient(cancellationToken);
        return await client.ExecuteAsync<string>(new Command(RedisCommandName.Lcs, new object[] {key1, key2}));
    }

    /// <summary>
    /// 匹配两个字符串的相似程度 返回匹配成功的内容的长度
    /// </summary>
    /// <param name="key1"></param>
    /// <param name="key2"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<long> LcsWithLenAsync(string key1, string key2, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await using var client = await GetRedisClient(cancellationToken);
        return await client.ExecuteAsync<long>(new Command(RedisCommandName.Lcs, new object[] {key1, key2, "LEN"}));
    }
}