using System.Diagnostics;
using System.Text.Json;
using Microsoft.Extensions.DiagnosticAdapter;
using RedisNaruto.EventDatas;

namespace test.aspnetcpre;

/// <summary>
/// 诊断支持
/// </summary>
public class RedisNarutoListenerAdapter_Case1 : IObserver<DiagnosticListener>,IObserver<KeyValuePair<string, object>>
{
    public void OnCompleted()
    {
        
    }

    public void OnError(Exception error)
    {
    }

    public void OnNext(KeyValuePair<string, object> value)
    {
        //判断是否为指定的消息
        if (value is {Key: "RedisNaruto:WriteRedisNarutoBefore", Value: WriteRedisNarutoMessageBeforeEventData eventData})
        {
            Console.WriteLine($"消息发送前 CMD={(eventData.Cmd)}");
        }
        if (value is {Key: "RedisNaruto:WriteRedisNarutoAfter", Value: WriteRedisNarutoMessageAfterEventData eventData2})
        {
            Console.WriteLine($"消息发送后 CMD={(eventData2.Cmd)},Result={eventData2.Result}");
        }
        if (value is {Key: "RedisNaruto:SelectRedisClientError", Value: SelectRedisClientErrorEventData eventData3})
        {
            Console.WriteLine($"选择客户端错误 Host={(eventData3.Host)},Port={eventData3.Port}，EventName={eventData3.EventName}");
        }

        {
            if (value is {Key: "RedisNaruto:BeginTran", Value: BeginTranEventData eventData6})
            {
                Console.WriteLine($"开启事务 Host={(eventData6.Host)},Port={eventData6.Port}，EventName={eventData6.EventName}");
            }
        }
        {
            if (value is {Key: "RedisNaruto:EndTran", Value: EndTranEventData eventData6})
            {
                Console.WriteLine($"执行事务 Host={(eventData6.Host)},Port={eventData6.Port}，EventName={eventData6.EventName}");
            }
        }
        {
            if (value is {Key: "RedisNaruto:DiscardTran", Value: DiscardTranEventData eventData6})
            {
                Console.WriteLine($"取消事务 Host={(eventData6.Host)},Port={eventData6.Port}，EventName={eventData6.EventName}");
            }
        }
    }

    public void OnNext(DiagnosticListener value)
    {
        if (value.Name == "RedisNarutoDiagnosticListener")
        {
            //订阅主题
            value.Subscribe(this);
        }
    }
}