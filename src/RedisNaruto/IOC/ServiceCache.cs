namespace RedisNaruto.IOC
{
    /// <summary>
    /// 服务注册信息缓存
    /// </summary>
    public class ServiceCache
    {
        private static readonly IDictionary<Type, ServiceInfo> Cache = new Dictionary<Type, ServiceInfo>();

        /// <summary>
        /// 获取服务
        /// </summary>
        /// <param name="implType"></param>
        /// <returns></returns>
        public static ServiceInfo Get(Type implType)
        {
            if (Cache.TryGetValue(implType, out var serviceInfo))
            {
                return serviceInfo;
            }

            throw new InvalidOperationException("类型没有注册");
        }

        /// <summary>
        /// 获取生命周期
        /// </summary>
        /// <param name="implType"></param>
        /// <returns></returns>
        public static Lifetime GetLifeTime(Type implType)
        {
            var serviceInfo = Get(implType);
            return serviceInfo.Lifetime;
        }

        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="serviceInfos"></param>
        internal static void Add(IEnumerable<ServiceInfo> serviceInfos)
        {
            foreach (var item in serviceInfos)
            {
                Cache.TryAdd(item.ImplType, item);
            }
        }
    }
}