using RedisNaruto.Internal.Enums;

namespace RedisNaruto.Internal.Attributes;

/// <summary>
/// Redis标识
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
internal class RedisIdentificationAttribute : Attribute
{
    /// <summary>
    /// 
    /// </summary>
    public RedisVersionSupportEnum Version { get;  }

    public TimeComplexityEnum TimeComplexity { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="version">版本</param>
    /// <param name="timeComplexity">时间复杂度</param>
    public RedisIdentificationAttribute(RedisVersionSupportEnum version, TimeComplexityEnum timeComplexity)
    {
        Version = version;
        TimeComplexity = timeComplexity;
    }
}