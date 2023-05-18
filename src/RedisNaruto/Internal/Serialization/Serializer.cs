using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using RedisNaruto.Utils;

namespace RedisNaruto.Internal.Serialization;

/// <summary>
/// 
/// </summary>
public sealed class Serializer : ISerializer
{
    public async Task<(byte[], int)> SerializeAsync(object source)
    {
        return source switch
        {
            byte[] sourceByte => (sourceByte, sourceByte.Length),
            DateOnly dateOnly => dateOnly.ToString("yyyy-MM-dd").ToEncodePool(),
            DateTime dateTime => dateTime.ToString("yyyy-MM-dd HH:mm:ss").ToEncodePool(),
            _ => Type.GetTypeCode(source.GetType()) switch
            {
                TypeCode.Object => await ToJsonAsync(source),
                _ => source.ToEncodePool()
            }
        };
        //获取类型编码
    }

    /// <summary>
    /// 转换json
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    private async Task<(byte[], int)> ToJsonAsync(object source)
    {
        await using var memoryStream = new MemoryStream();
        await JsonSerializer.SerializeAsync(memoryStream, source, BuildJsonSerializerOptions());
        return (memoryStream.ToArray(), 0);
    }

    public async Task<TResult> DeserializeAsync<TResult>(byte[] source)
    {
        if (source is not {Length: > 0})
        {
            return default;
        }

        await using var memoryStream = new MemoryStream(source);
        return await JsonSerializer.DeserializeAsync<TResult>(memoryStream);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private JsonSerializerOptions BuildJsonSerializerOptions()
    {
        return new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
            NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString,
            ReferenceHandler = ReferenceHandler.IgnoreCycles
        };
    }
}