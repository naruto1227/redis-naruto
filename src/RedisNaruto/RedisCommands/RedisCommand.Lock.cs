using RedisNaruto.RedisCommands.DistributedLocks;

namespace RedisNaruto.RedisCommands;

/// <summary>
/// 锁
/// </summary>
public partial class RedisCommand: IRedisCommand
{
   
    /// <summary>
    /// 上锁
    /// </summary>
    /// <param name="key"></param>
    /// <param name="expireTime"></param>
    /// <param name="waitTime"></param>
    /// <param name="delayTime"></param>
    /// <returns></returns>
    public async Task<IDistributedLock> CreateLockAsync(string key,TimeSpan expireTime, TimeSpan waitTime, TimeSpan delayTime)
    {
        return await DistributedLock.CreateAsync(this, key, expireTime, waitTime, delayTime);
    }
}