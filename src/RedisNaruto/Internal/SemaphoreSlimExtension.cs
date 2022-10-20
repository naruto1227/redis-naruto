namespace RedisNaruto.Internal;

internal static class SemaphoreSlimExtension
{
    /// <summary>
    /// 锁住资源
    /// </summary>
    /// <param name="semaphoreSlim"></param>
    /// <returns></returns>
    public static async Task<IDisposable> LockAsync(this SemaphoreSlim semaphoreSlim)
    {
        await semaphoreSlim.WaitAsync();
        return new DisposeAction(() => semaphoreSlim.Release());
    }
}