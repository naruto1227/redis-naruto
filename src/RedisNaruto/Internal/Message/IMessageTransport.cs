using System.IO.Pipelines;
using RedisNaruto.Exceptions;
using RedisNaruto.Internal.Models;
using RedisNaruto.Models;

namespace RedisNaruto.Internal.Message;

/// <summary>
/// 消息传输
/// </summary>
internal interface IMessageTransport
{
    /// <summary>
    /// 发送消息
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="command"></param>
    Task SendAsync(Stream stream, Command command);

    /// <summary>
    /// 读取消息
    /// </summary>
    /// <param name="stream"></param>
    /// <returns></returns>
    Task<object> ReceiveMessageAsync(Stream stream);

    /// <summary>
    /// 读取简易消息
    /// </summary>
    /// <param name="stream"></param>
    /// <returns></returns>
    /// <exception cref="NotSupportedException"></exception>
    /// <exception cref="RedisExecException"></exception>
    Task<RedisValue> ReceiveSimpleMessageAsync(Stream stream);
}