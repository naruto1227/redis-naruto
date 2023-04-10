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

    internal static RedisValue Null() => new RedisValue(default);

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


    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static implicit operator long(RedisValue value) => value.ToLong();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static implicit operator int(RedisValue value) => value.ToInt();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static implicit operator byte[](RedisValue value) => value.ToBytes;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static implicit operator string(RedisValue value) => value.ToString();

    // public static explicit operator RedisValue(string value) => new RedisValue(Encoding.Default.GetBytes(value));
    public static implicit operator RedisValue(string value) => new RedisValue(Encoding.Default.GetBytes(value));

    public static implicit operator RedisValue(byte[] value) => new RedisValue(value);

    public static implicit operator RedisValue(int value) =>
        new RedisValue(Encoding.Default.GetBytes(value.ToString()));

    public static implicit operator RedisValue(long value) =>
        new RedisValue(Encoding.Default.GetBytes(value.ToString()));
}