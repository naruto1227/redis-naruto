using System.Diagnostics;
using RedisNaruto.Enums;
using RedisNaruto.EventDatas;
using RedisNaruto.Internal.DiagnosticListeners;
using RedisNaruto.Models;

namespace RedisNaruto.RedisCommands.DistributedLocks;

/// <summary>
/// 实现
/// </summary>
public class DistributedLock : IDistributedLock
{
    /// <summary>
    /// 延长锁时间的定时器
    /// </summary>
    private Timer _extendedLockTimer;

    /// <summary>
    /// 
    /// </summary>
    private readonly IRedisCommand _redisCommand;

    /// <summary>
    /// 锁id
    /// </summary>
    public string LockId { get; }

    /// <summary>
    /// 资源名称
    /// </summary>
    public string ResourceName { get; }

    /// <summary>
    /// 过期时间
    /// </summary>
    public TimeSpan ExpireTime { get; }

    /// <summary>
    /// 是否成功 获取到锁
    /// </summary>
    public bool IsAcquired => Status == LockStatusEnum.Acquired;

    /// <summary>
    /// 锁的状态
    /// </summary>
    public LockStatusEnum Status { get; private set; }

    /// <summary>
    /// 重试次数
    /// </summary>
    private const int RetryCount = 3;

    /// <summary>
    /// 等待时间
    /// </summary>
    private TimeSpan WaitTime { get; }

    /// <summary>
    /// 轮询的延迟时间
    /// </summary>
    private TimeSpan DelayTime { get; }

    /// <summary>
    /// 默认的延迟等待时间
    /// </summary>
    private const long DefaultDelayTime = 100;

    /// <summary>
    /// 判断锁获取成功的服务数
    /// </summary>
    private int SuccessCount { get; set; }

    private static readonly string ExtendedTimeScript =
        AssemblyExtension.LoadManifestResource("RedisNaruto.Internal.Luas.ExtendedLockTime.lua");

    private static readonly string UnLockScript =
        AssemblyExtension.LoadManifestResource("RedisNaruto.Internal.Luas.UnLock.lua");

    /// <summary>
    /// 
    /// </summary>
    /// <param name="redisCommand"></param>
    /// <param name="resourceName"></param>
    /// <param name="expireTime"></param>
    /// <param name="waitTime"></param>
    /// <param name="delayTime"></param>
    private DistributedLock(IRedisCommand redisCommand, string resourceName,
        TimeSpan expireTime, TimeSpan waitTime, TimeSpan delayTime)
    {
        _redisCommand = redisCommand;
        LockId = CreateUniKey();
        ResourceName = resourceName;
        ExpireTime = expireTime;
        WaitTime = waitTime;
        DelayTime = delayTime;
        if (DelayTime.TotalMilliseconds <= 0)
        {
            DelayTime = TimeSpan.FromMilliseconds(DefaultDelayTime);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="expireTime">过期时间</param>
    ///<param name="resourceName">资源名称</param>
    ///<param name="delayTime">轮询的延迟等待时间</param>
    ///<param name="waitTime">获取锁的整体等待时间</param>
    /// <returns></returns>
    public static async Task<DistributedLock> CreateAsync(IRedisCommand redisCommand,
        string resourceName, TimeSpan expireTime, TimeSpan waitTime, TimeSpan delayTime)
    {
        //实例化对象
        var redLock = new DistributedLock(redisCommand, resourceName, expireTime, waitTime, delayTime);

        //资源获取成功的依据
        // redLock.SuccessCount = multiplexer.Count / 2 + 1;
        //执行锁
        await redLock.StartAsync().ConfigureAwait(false);
        return redLock;
    }

    /// <summary>
    /// 开启创建锁资源
    /// </summary>
    /// <returns></returns>
    private async Task StartAsync()
    {
        if (WaitTime.TotalMilliseconds > 0)
        {
            var stopwatch = Stopwatch.StartNew();
            while (!IsAcquired && stopwatch.Elapsed <= WaitTime)
            {
                //获取锁
                await AcquiredAsync().ConfigureAwait(false);
                if (!IsAcquired)
                    await Task.Delay(DelayTime).ConfigureAwait(false);
            }

            stopwatch.Stop();
        }
        else
            await AcquiredAsync();

        //开启新线程 定时延长锁时间
        if (IsAcquired)
        {
            RedisDiagnosticListener.LockCreateSuccess(ResourceName,LockId);
            AutoExtenedTime();
        }
        else
        {
            RedisDiagnosticListener.LockCreateFail(ResourceName,LockId);
        }
    }


    /// <summary>
    /// 获取锁
    /// </summary>
    /// <returns></returns>
    protected virtual async Task AcquiredAsync()
    {
        //重试
        for (var i = 1; i <= RetryCount; i++)
        {
            var stopwatch = Stopwatch.StartNew();
            var status= await LockAsync();

            stopwatch.Stop();
            //当有一半的数成功 并且 执行锁的消耗时间小于过期时间就代表锁获取成功
            // if (statusList.Count(a => a == LockStatusEnum.Acquired) >= SuccessCount &&
            //     stopwatch.Elapsed.TotalMilliseconds < ExpireTime.TotalMilliseconds)
            // {
            //     Status = LockStatusEnum.Acquired;
            //     return;
            // }
            if (status==LockStatusEnum.Acquired)
            {
                Status = LockStatusEnum.Acquired;
                return;
            }
            Status = LockStatusEnum.UnLock;
            // //取消锁
            // await ReleaseAsync().ConfigureAwait(false);

            await Task.Delay(TimeSpan.FromMilliseconds(50)).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public virtual async ValueTask DisposeAsync()
    {
        await DisposeAsync(true);
    }

    private void AutoExtenedTime()
    {
        //设置多久执行此定时器
        var time = ExpireTime.TotalMilliseconds / 2;
        //开启定时器
        _extendedLockTimer = new Timer((obj) => ExtendedTimeAsync(), null, (int) time, (int) time);
    }

    /// <summary>
    /// 扩展锁时间
    /// </summary>
    private async void ExtendedTimeAsync()
    {
        try
        {
            await _redisCommand.EvalAsync(ExtendedTimeScript,
                new object[]
                {
                    ResourceName
                }, new object[]
                {
                    LockId,
                    ExpireTime.TotalMilliseconds
                }).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            RedisDiagnosticListener.LockCreateException(ResourceName,LockId,ex);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="isDispose"></param>
    /// <returns></returns>
    protected virtual async ValueTask DisposeAsync(bool isDispose)
    {
        if (isDispose)
        {
            //释放锁
            await ReleaseAsync();
            if (_extendedLockTimer!=null)
            {
                //停止定时器
                _extendedLockTimer.Change(Timeout.Infinite, Timeout.Infinite);
                await _extendedLockTimer.DisposeAsync().AsTask()!;
                _extendedLockTimer = null;
            }
            Status = LockStatusEnum.UnLock;
            GC.SuppressFinalize(this);
        }
    }

    /// <summary>
    /// 释放锁资源
    /// </summary>
    /// <returns></returns>
    protected virtual async Task ReleaseAsync()
    {
        await UnLockAsync();
    }

    /// <summary>
    /// 加锁
    /// </summary>
    /// <returns></returns>
    private async Task<LockStatusEnum> LockAsync()
    {
        try
        {
            //锁定资源
            return await _redisCommand.SetNxAsync(ResourceName, LockId, ExpireTime)
                ? LockStatusEnum.Acquired
                : LockStatusEnum.UnLock;
        }
        catch (Exception ex)
        {
            //
           RedisDiagnosticListener.LockCreateException(ResourceName,LockId,ex);
            return LockStatusEnum.Error;
        }
    }

    /// <summary>
    /// 取消锁
    /// </summary>
    /// <returns></returns>
    private async Task UnLockAsync()
    {
        try
        {
            await _redisCommand.EvalAsync(UnLockScript,
                new object[]
                {
                    ResourceName
                }, new object[]
                {
                    LockId
                });
        }
        catch (Exception ex)
        {
            RedisDiagnosticListener.LockCreateException(ResourceName,LockId,ex);
        }
    }

    /// <summary>
    /// 生成一个唯一的密钥
    /// </summary>
    /// <returns></returns>
    private static string CreateUniKey() => Guid.NewGuid().ToString();
}

/// <summary>
/// 
/// </summary>
public class AssemblyExtension
{
    /// <summary>
    /// 加载嵌入的资源
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static string LoadManifestResource(string name)
    {
        using var resourceStream = typeof(AssemblyExtension).Assembly.GetManifestResourceStream(name);
        using var stringReader = new StreamReader(resourceStream);
        return stringReader.ReadToEnd();
    }
}