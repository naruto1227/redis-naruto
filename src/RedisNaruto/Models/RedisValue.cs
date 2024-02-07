using System.Buffers;
using System.Numerics;
using System.Text;
using RedisNaruto.Internal.Enums;
using RedisNaruto.Utils;

namespace RedisNaruto.Models;

/// <summary>
/// redis返回值 包装器
/// </summary>
public readonly struct RedisValue
{
    /// <summary>
    /// 消息类型
    /// </summary>
    public RespMessageTypeEnum MessageType { get; }

    /// <summary>
    /// 回复的值
    /// </summary>
    private readonly ReadOnlyMemory<byte> _memory;

    #region ctor

    internal RedisValue(ReadOnlyMemory<byte> memory, RespMessageTypeEnum respMessageType) : this(memory)
    {
        this.MessageType = respMessageType;
    }

    private RedisValue(byte[] bytes)
    {
        _memory = new ReadOnlyMemory<byte>(bytes);
        MessageType = RespMessageTypeEnum.Default;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="memory"></param>
    private RedisValue(ReadOnlyMemory<byte> memory)
    {
        _memory = memory;
        MessageType = RespMessageTypeEnum.Default;
    }

    #endregion


    internal static RedisValue Null() => new();

    /// <summary>
    /// 判断返回值是否为空
    /// </summary>
    /// <returns></returns>
    public bool IsEmpty() => Length <= 0;

    /// <summary>
    /// 是否错误
    /// </summary>
    public bool IsError => MessageType is RespMessageTypeEnum.Error or RespMessageTypeEnum.BuckError;

    /// <summary>
    /// 数据长度
    /// </summary>
    public long Length
    {
        get
        {
            if (_memory.IsEmpty)
            {
                return 0;
            }

            return _memory.Length;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override string ToString() => Encoding.UTF8.GetString(_memory.Span);

    /// <summary>
    /// 转成字节
    /// </summary>
    public byte[] ToBytes => _memory.ToArray();

    /// <summary>
    /// 转成字节
    /// </summary>
    public ReadOnlyMemory<byte> ToReadOnlyMemory => _memory;


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

    public int ToInt()
    {
        return IsEmpty() ? 0 : ToString().ToInt();
    }

    public long ToLong()
    {
        return IsEmpty() ? 0L : ToString().ToLong();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public double ToDouble()
    {
        if (IsEmpty())
        {
            return 0;
        }

        var dd = ToString();
        //判断是否为resp3中返回的
        return dd switch
        {
            "inf" =>
                //正无穷
                double.PositiveInfinity,
            "-inf" =>
                //负无穷
                double.NegativeInfinity,
            _ => dd.Todouble()
        };
    }

    /// <summary>
    /// 获取大数
    /// </summary>
    /// <returns></returns>
    public BigInteger ToBigInteger()
    {
        return IsEmpty() ? 0 : ToString().ToBigInteger();
    }

    /// <summary>
    /// 读取bool
    /// </summary>
    /// <param name="stream"></param>
    /// <returns></returns>
    public bool ToBool()
    {
        if (IsEmpty())
        {
            return default;
        }

        string dd = ToString();

        return dd switch
        {
            "t" => true,
            "f" => false,
            _ => dd.ToBool()
        };
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static implicit operator BigInteger(RedisValue value) => value.ToBigInteger();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static implicit operator bool(RedisValue value) => value.ToBool();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static implicit operator double(RedisValue value) => value.ToDouble();

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
    public static implicit operator RedisValue(string value) => new(value.ToEncode());

    public static implicit operator RedisValue(byte[] value) => new(value);

    public static implicit operator RedisValue(int value) =>
        new(value.ToEncode());

    public static implicit operator RedisValue(long value) =>
        new(value.ToEncode());
}