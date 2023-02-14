using RedisNaruto.Internal;
using RedisNaruto.Internal.Models;

namespace RedisNaruto.RedisCommands;

/// <summary>
/// hash操作
/// </summary>
public partial class RedisCommand : IRedisCommand
{
    /// <summary>
    /// hash删除
    /// </summary>
    /// <param name="key"></param>
    /// <param name="fields"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<long> HDelAsync(string key, string[] fields, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await using var client = await GetRedisClient(cancellationToken);
        return
            await client.ExecuteAsync<long>(new Command(RedisCommandName.HDel,
                new object[] {key}.Union(fields).ToArray()));
    }

    /// <summary>
    /// hash存储
    /// </summary>
    /// <param name="key"></param>
    /// <param name="fields"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<long> HSetAsync(string key, Dictionary<string, object> fields,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await using var client = await GetRedisClient(cancellationToken);
        var argv = new object[fields.Count * 2 + 1];
        argv[0] = key;

        var idx = 1;
        foreach (var (itemKey, value) in fields)
        {
            argv[idx * 2 - 1] = itemKey;
            argv[idx * 2] = value;
            idx++;
        }

        return
            await client.ExecuteAsync<long>(new Command(RedisCommandName.HSet,
                argv));
    }

    /// <summary>
    /// hash存在
    /// </summary>
    /// <param name="key"></param>
    /// <param name="field"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<bool> HExistsAsync(string key, string field, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await using var client = await GetRedisClient(cancellationToken);
        return
            await client.ExecuteAsync<int>(new Command(RedisCommandName.HExists,
                new object[]
                {
                    key, field
                })) == 1;
    }

    /// <summary>
    /// hash获取
    /// </summary>
    /// <param name="key"></param>
    /// <param name="field"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<string> HGetAsync(string key, string field, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await using var client = await GetRedisClient(cancellationToken);
        return
            await client.ExecuteAsync<string>(new Command(RedisCommandName.HGet,
                new object[]
                {
                    key, field
                }));
    }

    /// <summary>
    /// 获取所有的hash数据
    /// </summary>
    /// <param name="key"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<Dictionary<string, string>> HGetAllAsync(string key,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await using var client = await GetRedisClient(cancellationToken);
        var resultList =
            client.ExecuteMoreResultAsync<string>(new Command(RedisCommandName.HGetAll,
                new object[]
                {
                    key
                }));
        //下标
        var i = 1;
        //上一个值
        var preName = "";
        var res = new Dictionary<string, string>();
        await foreach (var item in resultList.WithCancellation(cancellationToken))
        {
            //双数为值
            if (i % 2 == 0)
            {
                res[preName] = item;
            }
            //单数为key
            else
            {
                preName = item;
                res[item] = "";
            }

            i++;
        }

        return res;
    }

    /// <summary>
    /// hash 递增
    /// </summary>
    /// <param name="key"></param>
    /// <param name="field"></param>
    /// <param name="increment">递增的值</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<double> HIncrByAsync(string key, string field, double increment = 1,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await using var client = await GetRedisClient(cancellationToken);
        return
            await client.ExecuteAsync<double>(new Command(RedisCommandName.HIncrBy,
                new object[]
                {
                    key, field, increment
                }));
    }
}