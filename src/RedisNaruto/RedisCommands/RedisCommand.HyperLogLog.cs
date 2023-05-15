using RedisNaruto.Internal;
using RedisNaruto.Internal.Models;
using RedisNaruto.Models;

namespace RedisNaruto.RedisCommands;

/// <summary>
/// HyperLogLog 数据结构可用于仅使用少量常量内存来计算集合中的唯一元素，特别是每个 HyperLogLog 12k 字节（加上键本身的几个字节）。
/// 观察集的返回基数不准确，但近似标准误差为 0.81%。
/// </summary>
public partial class RedisCommand : IRedisCommand
{
    /// <summary>
    /// 添加
    /// </summary>
    /// <param name="key"></param>
    /// <param name="elements">元素</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<int> PfAddAsync(string key, object[] elements, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(key);
        if (elements is not {Length: > 0})
        {
            throw new ArgumentNullException(nameof(elements));
        }

        //参数信息
        var argv = new List<object>()
        {
            key
        };
        argv.AddRange(elements);

        cancellationToken.ThrowIfCancellationRequested();
        
        return await RedisResolver.InvokeAsync<RedisValue>(new Command(RedisCommandName.PfAdd, argv.ToArray()));
    }

    /// <summary>
    /// 返回元素的基数值
    /// </summary>
    /// <param name="keys"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<long> PfCountAsync(string[] keys, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(keys);
        cancellationToken.ThrowIfCancellationRequested();
        
        return await RedisResolver.InvokeAsync<RedisValue>(new Command(RedisCommandName.PfCount, keys));
    }

    /// <summary>
    /// 将多个 HyperLogLog 值合并为一个唯一值，该值将近似于观察到的源 HyperLogLog 结构集的并集的基数。
    /// </summary>
    /// <param name="keys">原始key</param>
    /// <param name="destKey">存储的目标key</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<bool> PfMergeAsync(string[] keys, string destKey, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(keys);
        ArgumentNullException.ThrowIfNull(destKey);
        cancellationToken.ThrowIfCancellationRequested();
        //参数信息
        var argv = new List<object>()
        {
            destKey
        };
        argv.AddRange(keys);
        
        return await RedisResolver.InvokeAsync<RedisValue>(new Command(RedisCommandName.PfMerge, argv.ToArray())) == "OK";
    }
}