using System.ComponentModel;

namespace RedisNaruto.Internal.Enums;

/// <summary>
/// 时间复杂度
/// </summary>
internal enum TimeComplexityEnum
{
    O1,
    On,
    [Description("O(N*M)")]
    Onm,
    [Description("O(N+M)")]
    On_m,
    OlogN,
    [Description("O(log(N)+M) with N being the number of elements in the sorted set and M the number of elements returned.")]
    OlogN_M,
    [Description("O(log(N)*M) with N being the number of elements in the sorted set, and M being the number of elements popped.")]
    OlogNM,
    [Description("O(L + (N-K)log(N)) worst case where L is the total number of elements in all the sets, N is the size of the first set, and K is the size of the result set.")]
    OL_N_K_logN,
    [Description("O(N)+O(M log(M)) with N being the sum of the sizes of the input sorted sets, and M being the number of elements in the resulting sorted set.")]
    ON_O_M_logM,
    [Description("O(N*K)+O(M*log(M)) worst case with N being the smallest input sorted set, K being the number of input sorted sets and M being the number of elements in the resulting sorted set.")]
    ONK_O_M_logM,
    OlogNlogN,
    UnKnow
}