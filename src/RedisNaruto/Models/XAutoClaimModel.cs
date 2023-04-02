namespace RedisNaruto.Models;

public struct XAutoClaimModel
{
    public XAutoClaimModel(string nextStartId, List<StreamEntityModel> entityModels, List<string> deletedIds)
    {
        NextStartId = nextStartId;
        EntityModels = entityModels;
        DeletedIds = deletedIds;
    }

    /// <summary>
    ///用作<start>下一次调用 的参数的流 ID 。
    /// </summary>
    public string NextStartId { get; }

    /// <summary>
    /// 成功转移的消息信息
    /// </summary>
    public List<StreamEntityModel> EntityModels { get; }

    /// <summary>
    /// 已经从流中删除的id信息
    /// 7.0.0
    /// </summary>
    public List<string> DeletedIds { get; }
}