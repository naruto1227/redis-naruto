namespace RedisNaruto.Internal;

/// <summary>
/// 异步释放
/// </summary>
public class AsyncDisposeAction : IAsyncDisposable
{
    private readonly Func<Task> _task;

    public AsyncDisposeAction(Func<Task> task)
    {
        _task = task;
    }

    public async ValueTask DisposeAsync()
    {
        await _task.Invoke();
    }
}