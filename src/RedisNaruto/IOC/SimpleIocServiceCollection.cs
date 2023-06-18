namespace RedisNaruto.IOC
{
    /// <summary>
    /// 服务集合信息
    /// </summary>
    public class SimpleIocServiceCollection
    {
        internal readonly IList<ServiceInfo> _services;

        public SimpleIocServiceCollection()
        {
            _services = new List<ServiceInfo>();
        }
        /// <summary>
        /// 添加服务信息
        /// </summary>
        /// <param name="serviceInfo"></param>
        public void Add(ServiceInfo serviceInfo)
        {
            _services.Add(serviceInfo);
        }
    }
}
