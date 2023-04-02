using System.Text;
using RedisNaruto.Utils;

namespace RedisNaruto.Models;

/// <summary>
/// redis返回值 包装器
/// </summary>
public readonly struct RedisValue
{
    /// <summary>
    /// 回复的值
    /// </summary>
    private readonly ReadOnlyMemory<byte> _memory;

    public RedisValue(ReadOnlyMemory<byte> memory)
    {
        _memory = memory;
    }

    /// <summary>
    /// 判断返回值是否为空
    /// </summary>
    /// <returns></returns>
    public bool IsEmpty() => _memory.IsEmpty;

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override string ToString() => Encoding.Default.GetString(_memory.ToArray());

    /// <summary>
    /// 转成字节
    /// </summary>
    public byte[] ToBytes => _memory.ToArray();

    public int ToInt()
    {
        return IsEmpty() ? 0 : ToString().ToInt();
    }

    public long ToLong()
    {
        return IsEmpty() ? 0L : ToString().ToLong();
    }

    public static bool operator ==(RedisValue x, string y)
    {
        if (x.IsEmpty())
        {
            return false;
        }

        return x.ToString() == y;
    }

    public static bool operator !=(RedisValue x, string y)
    {
        if (x.IsEmpty())
        {
            return false;
        }

        return x.ToString() != y;
    }
}