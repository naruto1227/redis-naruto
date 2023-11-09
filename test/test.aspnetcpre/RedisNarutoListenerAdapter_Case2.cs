using System.Diagnostics;
using Microsoft.Extensions.DiagnosticAdapter;
using RedisNaruto.EventDatas;

namespace test.aspnetcpre;

/// <summary>
/// 使用Microsoft.Extensions.DiagnosticAdapter 包
/// </summary>
public class RedisNarutoListenerAdapter_Case2 : IObserver<DiagnosticListener>
{
    [DiagnosticName("RedisNaruto:WriteRedisNarutoBefore")]
    public virtual void WriteRedisNarutoBefore(string cmd,object[] args)
    {
         Console.WriteLine($"消息发送前 {cmd}");
    }
    
    [DiagnosticName("RedisNaruto:WriteRedisNarutoAfter")]
    public virtual void WriteRedisNarutoBefore(string cmd,object[] args,object result)
    {
        Console.WriteLine($"消息发送后 {cmd}");
    }

    public void OnCompleted()
    {
        
    }

    public void OnError(Exception error)
    {
    }
    

    public void OnNext(DiagnosticListener value)
    {
        if (value.Name == "RedisNarutoDiagnosticListener")
        {
             value.SubscribeWithAdapter(this);
        }
    }
}