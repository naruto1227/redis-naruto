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
    public async Task<byte[]> SerializeAsync(object source)
    {
        return source switch
        {
            byte[] sourceByte => sourceByte,
            DateOnly dateOnly => dateOnly.ToString("yyyy-MM-dd").ToEncode(),
            DateTime dateTime => dateTime.ToString("yyyy-MM-dd HH:mm:ss").ToEncode(),
            _ => Type.GetTypeCode(source.GetType()) switch
            {
                TypeCode.Object => await ToJsonAsync(source),
                _ => source.ToEncode()
            }
        };
        //获取类型编码
    }

    /// <summary>
    /// 转换json
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    private async Task<byte[]> ToJsonAsync(object source)
    {
        await using var memoryStream = new MemoryStream();
        await JsonSerializer.SerializeAsync(memoryStream, source, BuildJsonSerializerOptions());
        return memoryStream.ToArray();
    }

    public async Task<TResult> DeserializeAsync<TResult>(byte[] source)
    {
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