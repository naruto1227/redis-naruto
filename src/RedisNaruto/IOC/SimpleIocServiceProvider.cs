namespace RedisNaruto.IOC
{
    internal sealed class SimpleIocServiceProvider : ISimpleIocServiceProvider
    {
        /// <summary>
        /// 
        /// </summary>
        private readonly ServiceProviderEngine _engine;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="root"></param>
        /// <param name="isRoot">是否为根节点创建的</param>
        internal SimpleIocServiceProvider(ServiceProviderEngine root, bool isRoot = false)
        {
            if (isRoot)
            {
                _engine = new ServiceProviderEngine(default, root.EngineScope);
            }
            else
            {
                var scope = new ServiceProviderEngineScope();
                _engine = new ServiceProviderEngine(root.EngineScope, scope);
            }
        }

        /// <summary>
        /// 获取服务 
        /// </summary>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        public object GetService(Type serviceType)
        {
            return _engine.GetService(serviceType);
        }

        /// <summary>
        /// 创建一个新的作用域
        /// </summary>
        /// <returns></returns>
        public ISimpleIocServiceProvider CreateScope()
        {
            return new SimpleIocServiceProvider(this._engine);
        }

        public void Dispose()
        {
            _engine?.Dispose();
        }
    }
}