namespace RedisNaruto.Internal;

/// <summary>
/// 资源释放
/// </summary>
internal class DisposeAction : IDisposable
{
    private readonly Action _action;

    public DisposeAction(Action action)
    {
        _action = action;
    }

    public void Dispose()
    {
        _action.Invoke();
    }
}