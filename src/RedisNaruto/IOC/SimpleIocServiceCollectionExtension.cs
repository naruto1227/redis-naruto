namespace RedisNaruto.IOC
{
    /// <summary>
    /// 
    /// </summary>
    public static class SimpleIocServiceCollectionExtension
    {
        /// <summary>
        /// 注册单例
        /// </summary>
        /// <typeparam name="ImplType"></typeparam>
        /// <typeparam name="ServiceType"></typeparam>
        /// <param name="serviceCollection"></param>
        public static SimpleIocServiceCollection AddSingleton<ImplType, ServiceType>(this SimpleIocServiceCollection serviceCollection) where ServiceType : ImplType
        {
            serviceCollection.Add(new ServiceInfo()
            {
                ImplType = typeof(ImplType),
                ServiceType = typeof(ServiceType),
                Lifetime = Lifetime.Singleton
            });
            return serviceCollection;
        }
        /// <summary>
        /// 注册Scoped
        /// </summary>
        /// <typeparam name="ImplType"></typeparam>
        /// <typeparam name="ServiceType"></typeparam>
        /// <param name="serviceCollection"></param>
        public static SimpleIocServiceCollection AddScoped<ImplType, ServiceType>(this SimpleIocServiceCollection serviceCollection) where ServiceType : ImplType
        {
            serviceCollection.Add(new ServiceInfo()
            {
                ImplType = typeof(ImplType),
                ServiceType = typeof(ServiceType),
                Lifetime = Lifetime.Scoped
            });
            return serviceCollection;
        }
        /// <summary>
        /// 注册Transient
        /// </summary>
        /// <typeparam name="ImplType"></typeparam>
        /// <typeparam name="ServiceType"></typeparam>
        /// <param name="serviceCollection"></param>
        public static SimpleIocServiceCollection AddTransient<ImplType, ServiceType>(this SimpleIocServiceCollection serviceCollection) where ServiceType : ImplType
        {
            serviceCollection.Add(new ServiceInfo()
            {
                ImplType = typeof(ImplType),
                ServiceType = typeof(ServiceType),
                Lifetime = Lifetime.Transient
            });
            return serviceCollection;
        }

        /// <summary>
        /// 构建容器提供者
        /// </summary>
        /// <param name="serviceCollection"></param>
        /// <returns></returns>
        public static ISimpleIocServiceProvider BuildServiceProvider(this SimpleIocServiceCollection serviceCollection)
        {
            //初始化对象的生命周期信息
            ServiceCache.Add(serviceCollection._services);
            //创建一个根服务
            var root = new ServiceProviderEngineScope();
            return new SimpleIocServiceProvider(new ServiceProviderEngine(root), true);
        }
    }
}
