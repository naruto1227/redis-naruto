using RedisNaruto.EventDatas;
using RedisNaruto.Models;
using RedisNaruto.Utils;

namespace RedisNaruto.Internal.Interceptors;

internal sealed class ClientSideCachingInterceptor
{
    public ClientSideCachingInterceptor(ClientSideCachingOption clientSideCachingOption)
    {
        _clientSideCachingOption = clientSideCachingOption;
    }

    /// <summary>
    ///  客户端缓存配置
    /// </summary>
    private ClientSideCachingOption _clientSideCachingOption;

    public void CommandBefore(object? sender, InterceptorCommandBeforeEventArgs e)
    {
        //
        if (e.Command.Key.IsNullOrWhiteSpace())
        {
            return;
        }

        // 判断是否需要缓存
        var value = ClientSideCachingDic.Get(e.Command.Key);
        if (value == null)
        {
            return;
        }

        e.SetRetValue(value);
    }

    public void CommandAfter(object? sender, InterceptorCommandAfterEventArgs e)
    {
        if (e.Command.Key.IsNullOrWhiteSpace())
        {
            return;
        }

        if (_clientSideCachingOption.KeyPrefix.Any(a => a.StartsWith(e.Command.Key)))
        {
            ClientSideCachingDic.Set(e.Command.Key, e.Value, _clientSideCachingOption.TimeOut);
        }
    }
}