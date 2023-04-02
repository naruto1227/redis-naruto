namespace RedisNaruto.Models;

/// <summary>
/// 流信息
/// </summary>
public struct StreamModel
{
    internal StreamModel(string key)
    {
        Key = key;
        StreamEntitys = new List<StreamEntityModel>();
    }

    /// <summary>
    /// 流的key
    /// </summary>
    public string Key { get; init; }

    /// <summary>
    /// 
    /// </summary>
    public List<StreamEntityModel> StreamEntitys { get; private set; }
}