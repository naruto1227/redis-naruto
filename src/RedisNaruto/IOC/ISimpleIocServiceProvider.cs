namespace RedisNaruto.IOC
{
    /// <summary>
    /// 容器提供者
    /// </summary>
    public interface ISimpleIocServiceProvider : IServiceProvider, IDisposable
    {
        /// <summary>
        /// 创建一个新的作用域
        /// </summary>
        /// <returns></returns>
        ISimpleIocServiceProvider CreateScope();
    }
}