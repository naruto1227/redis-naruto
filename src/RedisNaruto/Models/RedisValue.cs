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

    /// <summary>
    /// 
    /// </summary>
    private readonly object _objectValue;

    #region ctor

    internal RedisValue(ReadOnlyMemory<byte> memory, RespMessageTypeEnum respMessageType) : this(memory)
    {
        this.MessageType = respMessageType;
    }
    /// <summary>
    /// RESP3 使用的
    /// </summary>
    /// <param name="objectValue"></param>
    /// <param name="respMessageType"></param>
    internal RedisValue(object objectValue, RespMessageTypeEnum respMessageType)
    {
        this._objectValue = objectValue;
        this.MessageType = respMessageType;
    }

    private RedisValue(byte[] bytes)
    {
        _memory = new ReadOnlyMemory<byte>(bytes);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="memory"></param>
    private RedisValue(ReadOnlyMemory<byte> memory)
    {
        _memory = memory;
    }

    #endregion


    internal static RedisValue Null() => new();

    /// <summary>
    /// 错误消息
    /// </summary>
    /// <param name="memory"></param>
    /// <returns></returns>
    internal static RedisValue Error(ReadOnlyMemory<byte> memory) => new(memory, RespMessageTypeEnum.Error);

    /// <summary>
    /// 判断返回值是否为空
    /// </summary>
    /// <returns></returns>
    public bool IsEmpty() => MessageType == RespMessageTypeEnum.Default;

    /// <summary>
    /// 是否错误
    /// </summary>
    public bool IsError => MessageType == RespMessageTypeEnum.Error;

    /// <summary>
    /// 数据长度
    /// </summary>
    public long Length
    {
        get
        {
            //todo 判断没有写入的时候 _memory 是否为空
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


    #region RESP3 

    //todo 使用同一套api 进行调用  内部进行判断是RESP2 还是RESP3 ，对于调用感无感
    /// <summary>
    /// 转换RESP3的 maps
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public Dictionary<string, object> Resp3ToMaps()
    {
        //
        if (MessageType != RespMessageTypeEnum.Maps)
        {
            throw new InvalidOperationException($"当前消息类型不是{nameof(RespMessageTypeEnum.Maps)}");
        }

        return (Dictionary<string, object>) _objectValue;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public double Resp3ToDouble()
    {
        //
        if (MessageType != RespMessageTypeEnum.Double)
        {
            throw new InvalidOperationException($"当前消息类型不是{nameof(RespMessageTypeEnum.Double)}");
        }

        return (double) _objectValue;
    }

    /// <summary>
    /// 获取大数
    /// </summary>
    /// <returns></returns>
    public BigInteger Resp3ToBigInteger()
    {
        //
        if (MessageType != RespMessageTypeEnum.BigNumber)
        {
            throw new InvalidOperationException($"当前消息类型不是{nameof(RespMessageTypeEnum.BigNumber)}");
        }

        return (BigInteger) _objectValue;
    }
    #endregion
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
        new RedisValue(value.ToEncode());

    public static implicit operator RedisValue(long value) =>
        new RedisValue(value.ToEncode());
}