using System.ComponentModel;

namespace RedisNaruto.Models;

public struct XStreamInfoModel
{
    public XStreamInfoModel(long length, long radixTreeKeys, long radixTreeNodes, string lastGeneratedId,
        string maxDeletedEntryId, long entriesAdded, string recordedFirstEntryId, int groupCounts)
    {
        Length = length;
        RadixTreeKeys = radixTreeKeys;
        RadixTreeNodes = radixTreeNodes;
        LastGeneratedId = lastGeneratedId;
        MaxDeletedEntryId = maxDeletedEntryId;
        EntriesAdded = entriesAdded;
        RecordedFirstEntryId = recordedFirstEntryId;
        GroupCounts = groupCounts;
    }

    /// <summary>
    /// 流的长度
    /// </summary>
    [Description("length")]
    public long Length { get; set; }

    /// <summary>
    /// 底层基数数据结构中的键数
    /// </summary>
    /// <returns></returns>

    [Description("radix-tree-keys")]
    public long RadixTreeKeys { get; set; }

    /// <summary>
    /// 底层基数数据结构中的节点数
    /// </summary>
    [Description("radix-tree-nodes")]
    public long RadixTreeNodes { get; set; }

    /// <summary>
    /// 添加到流中的最新一条记录id
    /// </summary>
    [Description("last-generated-id")]
    public string LastGeneratedId { get; set; }

    /// <summary>
    /// 从流中删除的最大条目 ID
    /// </summary>
    [Description("max-deleted-entry-id")]
    public string MaxDeletedEntryId { get; set; }

    /// <summary>
    /// 在其生命周期内添加到流中的所有条目的计数
    /// </summary>
    [Description("entries-added")]
    public long EntriesAdded { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [Description("recorded-first-entry-id")]
    public string RecordedFirstEntryId { get; set; }

    /// <summary>
    /// 流内的消费者组数量
    /// </summary>
    [Description("groups")]
    public int GroupCounts { get; set; }
}