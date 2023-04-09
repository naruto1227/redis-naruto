using System.Runtime.CompilerServices;
using RedisNaruto.Internal;
using RedisNaruto.Internal.Models;
using RedisNaruto.Models;
using RedisNaruto.Utils;

namespace RedisNaruto.RedisCommands;

/// <summary>
/// list 数据类型
/// </summary>
public partial class RedisCommand : IRedisCommand
{
    /// <summary>
    /// 往list的指定元素 前或者后插入新的元素
    /// 需要先创建元素
    /// </summary>
    /// <param name="key"></param>
    /// <param name="pivot">目标元素</param>
    /// <param name="element">具体插入的元素</param>
    /// <param name="isBefore">是否插入到目标元素前</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<int> LInsertAsync(string key, object pivot, object element, bool isBefore,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await using var client = await GetRedisClient(cancellationToken);
        var result =
            await client.ExecuteAsync(new Command(RedisCommandName.LInsert, new object[]
            {
                key,
                isBefore ? "BEFORE" : "AFTER",
                pivot,
                element
            }));
        return result;
    }

    /// <summary>
    /// 往指定的下标处插入元素
    /// 需要先创建元素
    /// </summary>
    /// <param name="key"></param>
    /// <param name="index">插入的下标位置</param>
    /// <param name="element">具体插入的元素</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<bool> LSetAsync(string key, int index, object element,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await using var client = await GetRedisClient(cancellationToken);
        var result =
            await client.ExecuteAsync(new Command(RedisCommandName.LSet, new object[]
            {
                key,
                index,
                element
            }));
        return result == "OK";
    }

    /// <summary>
    /// 将所有指定值插入存储在 的列表的尾部key。如果key不存在，则在执行推送操作之前将其创建为空列表。当key持有一个不是列表的值时，返回一个错误。
    /// </summary>
    /// <param name="key"></param>
    /// <param name="element">具体插入的元素</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<int> RPushAsync(string key, object[] element,
        CancellationToken cancellationToken = default)
    {
        if (element is not {Length: > 0})
        {
            return default;
        }

        cancellationToken.ThrowIfCancellationRequested();
        await using var client = await GetRedisClient(cancellationToken);
        var result =
            await client.ExecuteAsync(new Command(RedisCommandName.RPush, new object[]
            {
                key
            }.Concat(element).ToArray()));
        return result;
    }

    /// <summary>
    /// 将元素插入到列表的尾部，当key存在的时候才插入
    /// </summary>
    /// <param name="key"></param>
    /// <param name="element">具体插入的元素</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<int> RPushxAsync(string key, object[] element,
        CancellationToken cancellationToken = default)
    {
        if (element is not {Length: > 0})
        {
            return default;
        }

        cancellationToken.ThrowIfCancellationRequested();
        await using var client = await GetRedisClient(cancellationToken);
        var result =
            await client.ExecuteAsync(new Command(RedisCommandName.RPushx, new object[]
            {
                key
            }.Concat(element).ToArray()));
        return result;
    }

    /// <summary>
    /// 将所有指定值插入存储在 的列表的头部key。如果key不存在，则在执行推送操作之前将其创建为空列表。当key持有一个不是列表的值时，返回一个错误。
    /// </summary>
    /// <param name="key"></param>
    /// <param name="element">具体插入的元素</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<int> LPushAsync(string key, object[] element,
        CancellationToken cancellationToken = default)
    {
        if (element is not {Length: > 0})
        {
            return default;
        }

        cancellationToken.ThrowIfCancellationRequested();
        await using var client = await GetRedisClient(cancellationToken);
        var result =
            await client.ExecuteAsync(new Command(RedisCommandName.LPush, new object[]
            {
                key
            }.Concat(element).ToArray()));
        return result;
    }

    /// <summary>
    /// 将元素插入到列表的头部，当key存在的时候才插入
    /// </summary>
    /// <param name="key"></param>
    /// <param name="element">具体插入的元素</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<int> LPushxAsync(string key, object[] element,
        CancellationToken cancellationToken = default)
    {
        if (element is not {Length: > 0})
        {
            return default;
        }

        cancellationToken.ThrowIfCancellationRequested();
        await using var client = await GetRedisClient(cancellationToken);
        var result =
            await client.ExecuteAsync(new Command(RedisCommandName.LPushx, new object[]
            {
                key
            }.Union(element).ToArray()));
        return result;
    }

    /// <summary>
    /// 修剪现有列表，使其仅包含指定范围的指定元素。和start都是stop从零开始的索引，其中0是列表的第一个元素（头），1下一个元素等等。
    /// </summary>
    /// <param name="key"></param>
    /// <param name="start">开始的下标 如果为负数的话，代表倒数第几</param>
    /// <param name="end">结束的下标 如果为负数的话，代表倒数第几</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<bool> LTrimAsync(string key, int start, int end,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await using var client = await GetRedisClient(cancellationToken);
        var result =
            await client.ExecuteAsync(new Command(RedisCommandName.LTrim, new object[]
            {
                key,
                start,
                end
            }));
        return result == "OK";
    }

    /// <summary>
    /// 移除并返回存储在 的列表的最后一个元素key。
    /// 默认情况下，该命令从列表末尾弹出一个元素。当提供可选count参数时，回复将由最多count元素组成，具体取决于列表的长度。
    /// </summary>
    /// <param name="key"></param>
    /// <param name="count">返回的消息条数</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async IAsyncEnumerable<RedisValue> RPopAsync(string key, int count = 1,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await using var client = await GetRedisClient(cancellationToken);
        var resultList =
            client.ExecuteMoreResultAsync(new Command(RedisCommandName.RPop, new object[]
            {
                key,
                count
            }));
        await foreach (var item in resultList.WithCancellation(cancellationToken))
        {
            yield return (RedisValue) item;
        }
    }

    /// <summary>
    /// 阻塞
    /// 移除并返回存储在 的列表的最后一个元素key。
    /// 默认情况下，该命令从列表末尾弹出一个元素
    /// </summary>
    /// <param name="key"></param>
    /// <param name="timeout">超时时间</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<(string key, RedisValue value)> BRPopAsync(string[] key, TimeSpan timeout,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await using var client = await GetRedisClient(cancellationToken);
        var result = await
            client.ExecuteWithObjectAsync(new Command(RedisCommandName.BRPop, key.Concat(new object[]
            {
                timeout.TotalSeconds
            }).ToArray()));
        //判断是否有数据
        if (result is not List<object> {Count: >= 2} resultList)
            return default;
        return (resultList[0].ToString(), (RedisValue) resultList[1]);
    }

    /// <summary>
    /// 移除并返回存储在 的列表的头部一个元素key。
    /// 默认情况下，该命令从列表头部弹出一个元素。当提供可选count参数时，回复将由最多count元素组成，具体取决于列表的长度。
    /// </summary>
    /// <param name="key"></param>
    /// <param name="count">返回的消息条数</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async IAsyncEnumerable<RedisValue> LPopAsync(string key, int count = 1,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await using var client = await GetRedisClient(cancellationToken);
        var resultList =
            client.ExecuteMoreResultAsync(new Command(RedisCommandName.LPop, new object[]
            {
                key,
                count
            }));
        await foreach (var item in resultList.WithCancellation(cancellationToken))
        {
            yield return (RedisValue) item;
        }
    }

    /// <summary>
    /// 阻塞
    /// 移除并返回存储在 的列表的头部一个元素key。
    /// 默认情况下，该命令从列表头部弹出一个元素
    /// </summary>
    /// <param name="key"></param>
    /// <param name="timeout">超时时间</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<(string key, RedisValue value)> BLPopAsync(string[] key, TimeSpan timeout,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await using var client = await GetRedisClient(cancellationToken);
        var result = await
            client.ExecuteWithObjectAsync(new Command(RedisCommandName.BLPop, key.Concat(new object[]
            {
                timeout.TotalSeconds
            }).ToArray()));
        //判断是否有数据
        return result is not List<object> {Count: >= 2} resultList
            ? default
            : (resultList[0].ToString(), (RedisValue) resultList[1]);
    }

    /// <summary>
    /// 删除列表元素
    /// </summary>
    /// <param name="key"></param>
    /// <param name="count">count > 0：删除元素等于element从头到尾移动。count < 0：删除等于element从尾部移动到头部的元素。count = 0: 移除所有等于 的元素element。</param>
    /// <param name="element">元素</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<int> LRemAsync(string key, int count, object element,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await using var client = await GetRedisClient(cancellationToken);
        var result =
            await client.ExecuteAsync(new Command(RedisCommandName.LRem, new object[]
            {
                key,
                count,
                element
            }));
        return result;
    }

    /// <summary>
    /// 查询集合信息
    /// </summary>
    /// <param name="key"></param>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async IAsyncEnumerable<RedisValue> LRangeAsync(string key, int start, int end,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await using var client = await GetRedisClient(cancellationToken);
        var resultList =
            client.ExecuteMoreResultAsync(new Command(RedisCommandName.LRange, new object[]
            {
                key,
                start,
                end
            }));
        await foreach (var item in resultList.WithCancellation(cancellationToken))
        {
            yield return (RedisValue) item;
        }
    }

    /// <summary>
    /// 队列长度
    /// </summary>
    /// <param name="key"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<int> LLenAsync(string key,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await using var client = await GetRedisClient(cancellationToken);
        var result =
            await client.ExecuteAsync(new Command(RedisCommandName.LLen, new object[]
            {
                key
            }));
        return result;
    }

    /// <summary>
    /// 返回指定下标的元素
    /// </summary>
    /// <param name="key"></param>
    /// <param name="index">下标</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<RedisValue> LIndexAsync(string key, int index,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await using var client = await GetRedisClient(cancellationToken);
        var result =
            await client.ExecuteAsync(new Command(RedisCommandName.LIndex, new object[]
            {
                key,
                index
            }));
        return result;
    }

    /// <summary>
    /// 返回元素的下标位置
    /// </summary>
    /// <param name="key"></param>
    /// <param name="element">具体元素</param>
    /// <param name="maxLen">最大遍历次数</param>
    /// <param name="cancellationToken"></param>
    /// <param name="rank">RANK选项指定要返回的第一个元素的“排名”，以防有多个匹配项。等级 1 表示返回第一个匹配项，等级 2 表示返回第二个匹配项，依此类推</param>
    /// <param name="count">返回count 个 匹配成功的元素</param>
    /// <returns></returns>
    public async Task<List<RedisValue>> LPosAsync(string key, object element, int rank = 1, int count = 1,
        int maxLen = 0,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await using var client = await GetRedisClient(cancellationToken);
        var result =
            await client.ExecuteMoreResultAsync(new Command(RedisCommandName.LPos, new object[]
            {
                key,
                element,
                "RANK",
                rank,
                "COUNT",
                count,
                "MAXLEN",
                maxLen
            })).ToRedisValueListAsync();
        return result;
    }

    /// <summary>
    /// 从提供的键名列表中的第一个非空列表键中弹出一个或多个元素。
    /// </summary>
    /// <param name="key"></param>
    /// <param name="count">每个key返回的消息数</param>
    /// <param name="cancellationToken"></param>
    /// <param name="isLeft">是从左还是右 弹出</param>
    /// <returns></returns>
    public async Task<Dictionary<string, List<RedisValue>>> LmPopAsync(string[] key, bool isLeft = true, int count = 1,
        CancellationToken cancellationToken = default)
    {
        if (key is not {Length: > 0})
        {
            return default;
        }

        cancellationToken.ThrowIfCancellationRequested();
        await using var client = await GetRedisClient(cancellationToken);
        var result =
            await client.ExecuteWithObjectAsync(new Command(RedisCommandName.LmPop, new object[]
                {
                    key.Length
                }.Concat(key)
                .Concat(new object[]
                {
                    (isLeft ? "LEFT" : "RIGHT"),
                    "COUNT",
                    count
                }).ToArray()));
        //判断是否有数据
        if (result is not List<object> {Count: >= 2} resultList || resultList[1] is not List<object> valueList)
            return default;
        var res = new Dictionary<string, List<RedisValue>>(1)
        {
            {
                resultList[0]!.ToString(), valueList.ToRedisValueList()
            }
        };
        return res;
    }

    /// <summary>
    /// lMPop的阻塞版本
    /// 从提供的键名列表中的第一个非空列表键中弹出一个或多个元素。
    /// </summary>
    /// <param name="key"></param>
    /// <param name="count">每个key返回的消息数</param>
    /// <param name="cancellationToken"></param>
    /// <param name="timeout">超时时间</param>
    /// <param name="isLeft">是从左还是右 弹出</param>
    /// <returns></returns>
    public async Task<Dictionary<string, List<RedisValue>>> BlMPopAsync(string[] key, TimeSpan timeout,
        bool isLeft = true,
        int count = 1,
        CancellationToken cancellationToken = default)
    {
        if (key is not {Length: > 0})
        {
            return default;
        }

        cancellationToken.ThrowIfCancellationRequested();
        await using var client = await GetRedisClient(cancellationToken);
        var result =
            await client.ExecuteWithObjectAsync(new Command(RedisCommandName.BlMPop, new object[]
                {
                    timeout.TotalSeconds,
                    key.Length
                }.Concat(key)
                .Concat(new object[]
                {
                    (isLeft ? "LEFT" : "RIGHT"),
                    "COUNT",
                    count
                }).ToArray()));
        //判断是否有数据
        if (result is not List<object> {Count: >= 2} resultList || resultList[1] is not List<object> valueList)
            return default;
        var res = new Dictionary<string, List<RedisValue>>(1)
        {
            {
                resultList[0]!.ToString(), valueList.ToRedisValueList()
            }
        };
        return res;
    }

    /// <summary>
    /// 将 source 消息从左或者右 移除，并存储到 destination 元素中的 left 或者right
    /// </summary>
    /// <param name="isDestinationLeft">指定destinationKey 是从左还是从右写入</param>
    /// <param name="cancellationToken"></param>
    /// <param name="sourceKey">原始的key</param>
    /// <param name="destinationKey">目标的key</param>
    /// <param name="isSourceLeft">指定sourceKey 是从左还是从右读取</param>
    /// <returns></returns>
    public async Task<RedisValue> LMoveAsync(string sourceKey, string destinationKey,
        bool isSourceLeft = true, bool isDestinationLeft = true,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await using var client = await GetRedisClient(cancellationToken);
        var result =
            await client.ExecuteAsync(new Command(RedisCommandName.LMove, new object[]
            {
                sourceKey,
                destinationKey,
                isSourceLeft ? "LEFT" : "RIGHT",
                isDestinationLeft ? "LEFT" : "RIGHT",
            }));
        return result;
    }

    /// <summary>
    /// BLMOVE是 的阻塞变体LMOVE 当source为空时，Redis 将阻塞连接，直到另一个客户端推送到它或直到达到timeout
    /// 将 source 消息从左或者右 移除，并存储到 destination 元素中的 left 或者right
    /// </summary>
    /// <param name="isDestinationLeft">指定destinationKey 是从左还是从右写入</param>
    /// <param name="cancellationToken"></param>
    /// <param name="sourceKey">原始的key</param>
    /// <param name="destinationKey">目标的key</param>
    /// <param name="timeout">超时时间</param>
    /// <param name="isSourceLeft">指定sourceKey 是从左还是从右读取</param>
    /// <returns></returns>
    public async Task<RedisValue> BLMoveAsync(string sourceKey, string destinationKey, TimeSpan timeout,
        bool isSourceLeft = true, bool isDestinationLeft = true,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await using var client = await GetRedisClient(cancellationToken);
        var result =
            await client.ExecuteAsync(new Command(RedisCommandName.BLMove, new object[]
            {
                sourceKey,
                destinationKey,
                isSourceLeft ? "LEFT" : "RIGHT",
                isDestinationLeft ? "LEFT" : "RIGHT",
                timeout.TotalSeconds
            }));
        return result;
    }
}