using System.Diagnostics;
using RedisNaruto.EventDatas;

namespace RedisNaruto.Internal.DiagnosticListeners;

/// <summary>
/// 
/// </summary>
public static class RedisDiagnosticListener
{
    private static readonly DiagnosticListener DiagnosticListener = new(DiagnosticListenerName);

    /// <summary>
    /// 名称
    /// </summary>
    private const string DiagnosticListenerName = "RedisNarutoDiagnosticListener";

    private const string Prefix = "RedisNaruto:";
    public const string WriteRedisNarutoMessageBefore = Prefix + nameof(WriteRedisNarutoBefore);
    public const string WriteRedisNarutoMessageAfter = Prefix + nameof(WriteRedisNarutoAfter);
    public const string SelectRedisClientMessageError = Prefix + nameof(SelectRedisClientError);
    public const string BeginPipeMessage = Prefix + nameof(BeginPipe);
    public const string EndPipeMessage = Prefix + nameof(EndPipe);
    public const string ReceiveSubMessage = Prefix + nameof(ReceiveSub);
    public const string BeginTranMessage = Prefix + nameof(BeginTran);
    public const string EndTranMessage = Prefix + nameof(EndTran);
    public const string DiscardTranMessage = Prefix + nameof(DiscardTran);
    public const string SendSentinelMessage = Prefix + nameof(SendSentinel);
    public const string ReceiveSentinelMessage = Prefix + nameof(ReceiveSentinel);
    public const string SentinelError = Prefix + nameof(SentinelMessageError);
    public const string LockCreateSuccessMessage = Prefix + nameof(LockCreateSuccess);
    public const string LockCreateFailMessage = Prefix + nameof(LockCreateFail);
    public const string LockCreateExceptionMessage = Prefix + nameof(LockCreateException);

    public const string ClientSideCachingExceptionMessage = Prefix + nameof(ClientSideCachingException);
    public const string ClientSideCachingStartMessage = Prefix + nameof(ClientSideCachingStart);
    public const string ClientSideCachingRemoveMessage = Prefix + nameof(ClientSideCachingRemove);
    public const string ClientSideCachingUpdateMessage = Prefix + nameof(ClientSideCachingUpdate);

    /// <summary>
    /// 写入缓存前
    /// </summary>
    /// <returns></returns>
    internal static void WriteRedisNarutoBefore(string cmd, object[] args)
    {
        if (!DiagnosticListener.IsEnabled(WriteRedisNarutoMessageBefore)) return;
        DiagnosticListener.Write(WriteRedisNarutoMessageBefore, new WriteRedisNarutoMessageBeforeEventData(cmd, args));
    }

    /// <summary>
    /// 写入缓存后
    /// </summary>
    /// <returns></returns>
    internal static void WriteRedisNarutoAfter(string cmd, object[] args, object result)
    {
        if (!DiagnosticListener.IsEnabled(WriteRedisNarutoMessageAfter)) return;
        DiagnosticListener.Write(WriteRedisNarutoMessageAfter,
            new WriteRedisNarutoMessageAfterEventData(cmd, args, result));
    }

    /// <summary>
    /// 选择客户端触发的错误
    /// </summary>
    /// <returns></returns>
    internal static void SelectRedisClientError(string host, int port, Exception exception,
        string eventName = nameof(SelectRedisClientErrorEventData))
    {
        if (!DiagnosticListener.IsEnabled(SelectRedisClientMessageError)) return;
        DiagnosticListener.Write(SelectRedisClientMessageError,
            new SelectRedisClientErrorEventData(host, port, exception, eventName));
    }

    /// <summary>
    /// 开启流水线
    /// </summary>
    /// <returns></returns>
    internal static void BeginPipe(string host, int port)
    {
        if (!DiagnosticListener.IsEnabled(BeginPipeMessage)) return;
        DiagnosticListener.Write(BeginPipeMessage,  new BeginPipeEventData(host,port));
    }

    /// <summary>
    /// 结束流水线
    /// </summary>
    /// <returns></returns>
    internal static void EndPipe(string host, int port, object result)
    {
        if (!DiagnosticListener.IsEnabled(EndPipeMessage)) return;
        DiagnosticListener.Write(EndPipeMessage, new EndPipeEventData(host,port,result));
    }

    /// <summary>
    /// 接收订阅消息处理
    /// </summary>
    /// <returns></returns>
    internal static void ReceiveSub(object result)
    {
        if (!DiagnosticListener.IsEnabled(ReceiveSubMessage)) return;
        DiagnosticListener.Write(ReceiveSubMessage, new ReceiveSubMessageEventData(result));
    }

    /// <summary>
    /// 开启事务
    /// </summary>
    /// <returns></returns>
    internal static void BeginTran(string host, int port)
    {
        if (!DiagnosticListener.IsEnabled(BeginTranMessage)) return;
        DiagnosticListener.Write(BeginTranMessage, new BeginTranEventData(host,port));
    }

    /// <summary>
    /// 结束事务
    /// </summary>
    
    /// <returns></returns>
    internal static void EndTran(string host, int port, object result)
    {
        if (!DiagnosticListener.IsEnabled(EndTranMessage)) return;
        DiagnosticListener.Write(EndTranMessage, new EndTranEventData(host,port, result));
    }

    /// <summary>
    /// 取消事务
    /// </summary>
    /// <returns></returns>
    internal static void DiscardTran(string host, int port)
    {
        if (!DiagnosticListener.IsEnabled(DiscardTranMessage)) return;
        DiagnosticListener.Write(DiscardTranMessage,  new DiscardTranEventData(host,port));
    }

    /// <summary>
    /// 发送哨兵消息
    /// </summary>
    /// <returns></returns>
    internal static void SendSentinel(string host, int port, string cmd, object[] argv)
    {
        if (!DiagnosticListener.IsEnabled(SendSentinelMessage)) return;
        DiagnosticListener.Write(SendSentinelMessage, new SendSentinelMessageEventData(host, port, cmd,
            argv));
    }

    /// <summary>
    /// 接收哨兵消息
    /// </summary>
    /// <returns></returns>
    internal static void ReceiveSentinel(string host, int port, string cmd, object[] argv, object result)
    {
        if (!DiagnosticListener.IsEnabled(ReceiveSentinelMessage)) return;
        DiagnosticListener.Write(ReceiveSentinelMessage,  new ReceiveSentinelMessageEventData(host, port,
            cmd, argv, result));
    }

    /// <summary>
    ///哨兵错误
    /// </summary>
    /// <returns></returns>
    internal static void SentinelMessageError(string host, int port, Exception exception)
    {
        if (!DiagnosticListener.IsEnabled(SentinelError)) return;
        DiagnosticListener.Write(SentinelError,  new SentinelMessageErrorEventData(host,port,exception));
    }

    /// <summary>
    ///创建锁成功
    /// </summary>
    /// <returns></returns>
    internal static void LockCreateSuccess(string resourceName, string lockId)
    {
        if (!DiagnosticListener.IsEnabled(LockCreateSuccessMessage)) return;
        DiagnosticListener.Write(LockCreateSuccessMessage,  new LockCreateSuccessEventData(resourceName,lockId));
    }

    /// <summary>
    ///获取锁失败
    /// </summary>
    /// <returns></returns>
    internal static void LockCreateFail(string resourceName, string lockId)
    {
        if (!DiagnosticListener.IsEnabled(LockCreateFailMessage)) return;
        DiagnosticListener.Write(LockCreateFailMessage, new LockCreateFailEventData(resourceName,lockId));
    }

    /// <summary>
    ///锁报错
    /// </summary>
    /// <returns></returns>
    internal static void LockCreateException(string resourceName, string lockId, Exception exception)
    {
        if (!DiagnosticListener.IsEnabled(LockCreateExceptionMessage)) return;
        DiagnosticListener.Write(LockCreateExceptionMessage,  new LockCreateExceptionEventData(resourceName,lockId,exception));
    }

    ///  <summary>
    /// 客户端缓存执行异常
    ///  </summary>
    ///  <param name="clientId">客户端id</param>
    ///  <param name="result">返回结果</param>
    ///  <returns></returns>
    internal static void ClientSideCachingStart(string clientId, object result)
    {
        if (!DiagnosticListener.IsEnabled(ClientSideCachingStartMessage)) return;
        DiagnosticListener.Write(ClientSideCachingStartMessage, new ClientSideCachingStartEventData(clientId, result));
    }

    ///  <summary>
    /// 客户端缓存删除
    ///  </summary>
    ///  <param name="key">删除的key</param>
    ///  <returns></returns>
    internal static void ClientSideCachingRemove(string key)
    {
        if (!DiagnosticListener.IsEnabled(ClientSideCachingRemoveMessage)) return;
        DiagnosticListener.Write(ClientSideCachingRemoveMessage, new ClientSideCachingRemoveEventData(key));
    }

    ///  <summary>
    /// 客户端缓存更新
    ///  </summary>
    ///  <param name="key">删除的key</param>
    ///  <param name="value"></param>
    ///  <returns></returns>
    internal static void ClientSideCachingUpdate(string key, object value)
    {
        if (!DiagnosticListener.IsEnabled(ClientSideCachingUpdateMessage)) return;
        DiagnosticListener.Write(ClientSideCachingUpdateMessage, new ClientSideCachingUpdateEventData(key, value));
    }

    /// <summary>
    ///客户端缓存执行异常
    /// </summary>
    /// <param name="exception"></param>
    /// <returns></returns>
    internal static void ClientSideCachingException(Exception exception)
    {
        if (!DiagnosticListener.IsEnabled(ClientSideCachingExceptionMessage)) return;
        DiagnosticListener.Write(ClientSideCachingExceptionMessage, new ClientSideCachingExceptionEventData(exception));
    }
}