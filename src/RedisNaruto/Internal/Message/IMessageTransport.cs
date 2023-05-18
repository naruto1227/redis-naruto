using System.IO.Pipelines;
using RedisNaruto.Internal.Models;

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
    /// 接收消息
    /// </summary>
    /// <param name="stream"></param>
    /// <returns></returns>
    Task<PipeReader> ReceiveAsync(Stream stream);

    // /// <summary>
    // /// pipe 接收消息
    // /// </summary>
    // /// <param name="stream"></param>
    // /// <param name="pipeCount">管道的读取次数</param>
    // /// <returns></returns>
    // Task<object[]> PipeReceiveAsync(Stream stream, int pipeCount);
}