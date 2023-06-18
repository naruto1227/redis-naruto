namespace RedisNaruto.IOC
{
    /// <summary>
    /// 服务信息
    /// </summary>
    public class ServiceInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public Type ImplType { get; set; }
        /// <summary>
        /// 服务实现
        /// </summary>
        public Type ServiceType { get; set; }

        /// <summary>
        /// 生命周期
        /// </summary>
        public Lifetime Lifetime { get; set; }
    }
}
