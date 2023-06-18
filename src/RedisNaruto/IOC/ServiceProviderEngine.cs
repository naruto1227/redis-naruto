namespace RedisNaruto.IOC
{
    /// <summary>
    /// 
    /// </summary>
    public class ServiceProviderEngine : IDisposable
    {
        public ServiceProviderEngineScope EngineScope { get; set; }

        public ServiceProviderEngine(ServiceProviderEngineScope engineScope)
        {
            EngineScope = engineScope;
        }

        public ServiceProviderEngine(ServiceProviderEngineScope root, ServiceProviderEngineScope engineScope) : this(
            engineScope)
        {
            _root = root;
        }

        /// <summary>
        /// 
        /// </summary>
        private readonly ServiceProviderEngineScope _root;


        /// <summary>
        ///根容器
        /// </summary>
        private ServiceProviderEngineScope Root => _root ?? EngineScope;

        /// <summary>
        /// 获取服务
        /// </summary>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        public object GetService(Type serviceType)
        {
            //验证对象的生命周期
            return ServiceCache.GetLifeTime(serviceType) == Lifetime.Singleton
                ? Root.GetService(serviceType)
                : EngineScope.GetService(serviceType);
        }

        public void Dispose()
        {
            EngineScope?.Dispose();
        }
    }
}