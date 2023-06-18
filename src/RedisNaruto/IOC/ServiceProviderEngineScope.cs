using System.Collections.Concurrent;
using System.Reflection;

namespace RedisNaruto.IOC
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class ServiceProviderEngineScope
    {
        /// <summary>
        /// 缓存对象
        /// </summary>
        private ConcurrentDictionary<Type, object> _cacheObject;

        /// <summary>
        /// 需要释放的资源
        /// </summary>
        private IList<IDisposable> _disposables;

        /// <summary>
        /// 
        /// </summary>
        public ServiceProviderEngineScope()
        {
            _cacheObject = new ConcurrentDictionary<Type, object>();
            _disposables = new List<IDisposable>();
        }

        /// <summary>
        /// 获取服务
        /// </summary>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        public object GetService(Type serviceType)
        {
            //获取生命周期
            var serviceInfo = ServiceCache.Get(serviceType);
            return serviceInfo.Lifetime switch
            {
                Lifetime.Transient => NoCacheObject(serviceInfo.ServiceType),
                _ => CreateCacheObject(serviceInfo.ServiceType)
            };
        }

        /// <summary>
        /// 创建无缓存
        /// </summary>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        private object NoCacheObject(Type serviceType)
        {
            return CreateObject(serviceType);
        }

        /// <summary>
        /// 创建缓存对象
        /// </summary>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        private object CreateCacheObject(Type serviceType)
        {
            //优先读取缓存
            _cacheObject.TryGetValue(serviceType, out var service);
            if (service != null)
            {
                return service;
            }

            var obj = CreateObject(serviceType);
            //存储
            _cacheObject.TryAdd(serviceType, obj);
            return obj;
        }

        /// <summary>
        /// 创建对象
        /// </summary>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        private object CreateObject(Type serviceType)
        {
            //获取构造函数信息
            var constructors = serviceType.GetConstructors();
            if (constructors.Length == 0)
            {
                throw new InvalidOperationException("匹配不到构造函数");
            }

            //获取第一个构造函数
            var constructor = constructors[0];
            //获取构造函数的参数
            var parameters = constructor.GetParameters();
            //构建对象
            return BuildObject(constructor, parameters);
        }

        private object BuildObject(ConstructorInfo constructorInfo, ParameterInfo[] parameterInfos)
        {
            object obj = default;
            //无参
            if (parameterInfos.Length == 0)
            {
                obj = constructorInfo.Invoke(default);
            }
            //有参
            else
            {
                var parameterObj = new object[parameterInfos.Length];
                for (var i = 0; i < parameterObj.Length; i++)
                {
                    parameterObj[i] = GetService(parameterInfos[i].ParameterType);
                }

                obj = constructorInfo.Invoke(parameterObj);
            }

            if (obj is IDisposable disposable)
            {
                _disposables.Add(disposable);
            }

            return obj;
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool isDispose)
        {
            if (!isDispose) return;
            if (_disposables is {Count: > 0})
            {
                foreach (var item in _disposables)
                {
                    item.Dispose();
                }

                _disposables.Clear();
                _disposables = null;
            }

            _cacheObject?.Clear();
            _cacheObject = null;
            GC.SuppressFinalize(this);
        }
    }
}