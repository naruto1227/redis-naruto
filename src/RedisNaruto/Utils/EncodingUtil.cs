using System.Text;

namespace RedisNaruto.Utils;

/// <summary>
/// 编码
/// </summary>
internal static class EncodingUtil
{
    /// <summary>
    /// 编码
    /// </summary>
    /// <param name="obj"></param>
    public static byte[] ToEncode(this object obj)
    {
        return obj switch
        {
            byte[] bytes => bytes,
            string str => Encoding.Default.GetBytes(str),
            _ => Encoding.Default.GetBytes(obj + "")
        };
    }
}