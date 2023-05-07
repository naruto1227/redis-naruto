using RedisNaruto.Internal;
using RedisNaruto.Internal.Models;
using RedisNaruto.Models;
using RedisNaruto.Utils;

namespace RedisNaruto.RedisCommands;

/// <summary>
/// 服务
/// </summary>
public partial class RedisCommand : IRedisCommand
{
    /// <summary>
    /// 返回选中数据库的key数量
    /// </summary>
    /// <returns></returns>
    public async Task<long> DbSizeAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await using var client = await GetRedisClient(cancellationToken);
        return await client.ExecuteAsync(new Command(RedisCommandName.DbSize, default));
    }

    /// <summary>
    /// 慢日志查询
    /// </summary>
    /// <param name="count">返回的条数</param>
    /// <returns></returns>
    public async Task<SlowLogModel[]> SlowLogAsync(int count, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await using var client = await GetRedisClient(cancellationToken);
        var result = await client.ExecuteWithObjectAsync(new Command(RedisCommandName.SlowLog, new object[]
        {
            "Get",
            count
        }));
        if (result is not List<object> {Count: > 0} list)
        {
            return default;
        }

        var resultList = new SlowLogModel[list.Count];
        var i = 0;
        foreach (var item in list)
        {
            if (item is List<object> itemList)
            {
                resultList[i] = new SlowLogModel((RedisValue) itemList[0], (RedisValue) itemList[1],
                    (RedisValue) itemList[2],
                    ((List<object>) itemList[3]).ToRedisValueList(), (RedisValue) itemList[4],
                    (RedisValue) itemList[5]);
            }

            i++;
        }

        return resultList;
    }

    /// <summary>
    /// 慢日志的条数
    /// </summary>
    /// <returns></returns>
    public async Task<int> SlowLogLenAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await using var client = await GetRedisClient(cancellationToken);
        return await client.ExecuteAsync(new Command(RedisCommandName.SlowLog, new object[]
        {
            "LEN"
        }));
    }

    /// <summary>
    /// 清除慢日志记录
    /// </summary>
    /// <returns></returns>
    public async Task<bool> SlowLogResetAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await using var client = await GetRedisClient(cancellationToken);
        return await client.ExecuteAsync(new Command(RedisCommandName.SlowLog, new object[]
        {
            "RESET"
        })) == "OK";
    }
}