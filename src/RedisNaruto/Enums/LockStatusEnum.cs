namespace RedisNaruto.Enums;


/// <summary>
/// 锁的状态
/// </summary>
public enum LockStatusEnum
{
    /// <summary>
    /// 没有获取到锁
    /// </summary>
    UnLock,
    /// <summary>
    /// 获取到锁
    /// </summary>
    Acquired,
    /// <summary>
    /// 报错
    /// </summary>
    Error
}