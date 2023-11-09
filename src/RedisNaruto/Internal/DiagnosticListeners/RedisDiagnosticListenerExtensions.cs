using System.Diagnostics;
using RedisNaruto.EventDatas;

namespace RedisNaruto.Internal.DiagnosticListeners;

/// <summary>
/// 
/// </summary>
public static class RedisDiagnosticListenerExtensions
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
    public const string SentinelError= Prefix + nameof(SentinelMessageError);
    
    /// <summary>
    /// 写入缓存前
    /// </summary>
    /// <param name="eventData"></param>
    /// <returns></returns>
    internal static void WriteRedisNarutoBefore(this WriteRedisNarutoMessageBeforeEventData eventData)
    {
        if (!DiagnosticListener.IsEnabled(WriteRedisNarutoMessageBefore)) return;
        DiagnosticListener.Write(WriteRedisNarutoMessageBefore, eventData);
    }

    /// <summary>
    /// 写入缓存后
    /// </summary>
    /// <param name="eventData"></param>
    /// <returns></returns>
    internal static void WriteRedisNarutoAfter(this WriteRedisNarutoMessageAfterEventData eventData)
    {
        if (!DiagnosticListener.IsEnabled(WriteRedisNarutoMessageAfter)) return;
        DiagnosticListener.Write(WriteRedisNarutoMessageAfter, eventData);
    }
    
    /// <summary>
    /// 选择客户端触发的错误
    /// </summary>
    /// <param name="eventData"></param>
    /// <returns></returns>
    internal static void SelectRedisClientError(this SelectRedisClientErrorEventData eventData)
    {
        if (!DiagnosticListener.IsEnabled(SelectRedisClientMessageError)) return;
        DiagnosticListener.Write(SelectRedisClientMessageError, eventData);
    }
    
    /// <summary>
    /// 开启流水线
    /// </summary>
    /// <param name="eventData"></param>
    /// <returns></returns>
    internal static void BeginPipe(this BeginPipeEventData eventData)
    {
        if (!DiagnosticListener.IsEnabled(BeginPipeMessage)) return;
        DiagnosticListener.Write(BeginPipeMessage, eventData);
    }
    /// <summary>
    /// 结束流水线
    /// </summary>
    /// <param name="eventData"></param>
    /// <returns></returns>
    internal static void EndPipe(this EndPipeEventData eventData)
    {
        if (!DiagnosticListener.IsEnabled(EndPipeMessage)) return;
        DiagnosticListener.Write(EndPipeMessage, eventData);
    }
    /// <summary>
    /// 接收订阅消息处理
    /// </summary>
    /// <param name="eventData"></param>
    /// <returns></returns>
    internal static void ReceiveSub(this ReceiveSubMessageEventData eventData)
    {
        if (!DiagnosticListener.IsEnabled(ReceiveSubMessage)) return;
        DiagnosticListener.Write(ReceiveSubMessage, eventData);
    }
    /// <summary>
    /// 开启事务
    /// </summary>
    /// <param name="eventData"></param>
    /// <returns></returns>
    internal static void BeginTran(this BeginTranEventData eventData)
    {
        if (!DiagnosticListener.IsEnabled(BeginTranMessage)) return;
        DiagnosticListener.Write(BeginTranMessage, eventData);
    }
    /// <summary>
    /// 结束事务
    /// </summary>
    /// <param name="eventData"></param>
    /// <returns></returns>
    internal static void EndTran(this EndTranEventData eventData)
    {
        if (!DiagnosticListener.IsEnabled(EndTranMessage)) return;
        DiagnosticListener.Write(EndTranMessage, eventData);
    }
    /// <summary>
    /// 取消事务
    /// </summary>
    /// <param name="eventData"></param>
    /// <returns></returns>
    internal static void DiscardTran(this DiscardTranEventData eventData)
    {
        if (!DiagnosticListener.IsEnabled(DiscardTranMessage)) return;
        DiagnosticListener.Write(DiscardTranMessage, eventData);
    }
    /// <summary>
    /// 发送哨兵消息
    /// </summary>
    /// <param name="eventData"></param>
    /// <returns></returns>
    internal static void SendSentinel(this SendSentinelMessageEventData eventData)
    {
        if (!DiagnosticListener.IsEnabled(SendSentinelMessage)) return;
        DiagnosticListener.Write(SendSentinelMessage, eventData);
    }
    /// <summary>
    /// 接收哨兵消息
    /// </summary>
    /// <param name="eventData"></param>
    /// <returns></returns>
    internal static void ReceiveSentinel(this ReceiveSentinelMessageEventData eventData)
    {
        if (!DiagnosticListener.IsEnabled(ReceiveSentinelMessage)) return;
        DiagnosticListener.Write(ReceiveSentinelMessage, eventData);
    }
    /// <summary>
    ///哨兵错误
    /// </summary>
    /// <param name="eventData"></param>
    /// <returns></returns>
    internal static void SentinelMessageError(this SentinelMessageErrorEventData eventData)
    {
        if (!DiagnosticListener.IsEnabled(SentinelError)) return;
        DiagnosticListener.Write(SentinelError, eventData);
    }
    
}