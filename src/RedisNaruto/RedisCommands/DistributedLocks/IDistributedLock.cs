using RedisNaruto.Enums;

namespace RedisNaruto.RedisCommands.DistributedLocks;


/// <summary>
/// 分布式锁 接口对象
/// </summary>
public interface IDistributedLock : IAsyncDisposable
{
    /// <summary>
    /// 资源名称
    /// </summary>
    public string ResourceName { get; }
    /// <summary>
    /// 锁id
    /// </summary>
    public string LockId { get; }
    /// <summary>
    /// 过期时间
    /// </summary>
    public TimeSpan ExpireTime { get; }

    /// <summary>
    /// 是否成功 获取到锁
    /// </summary>
    public bool IsAcquired { get; }
    /// <summary>
    /// 锁的状态
    /// </summary>
    public LockStatusEnum Status { get; }
}