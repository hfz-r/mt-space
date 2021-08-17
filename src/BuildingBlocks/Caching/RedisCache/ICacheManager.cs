using System;
using System.Threading.Tasks;
using Key = AHAM.BuildingBlocks.RedisCache.CacheKey.CacheKey;

namespace AHAM.BuildingBlocks.RedisCache
{
    public interface ICacheManager : IDisposable
    {
        T Get<T>(Key key, Func<T> acquire);
        Task<T> GetAsync<T>(Key key, Func<Task<T>> acquire);
        Task<T> GetAsync<T>(Key key, Func<T> acquire);
        void Set(Key cache, object data);
        Task SetAsync(Key key, object data);
        Task RemoveAsync(Key cacheKey, params object[] cacheKeyParameters);
        Task RemoveByPrefixAsync(string prefix, params object[] prefixParameters);
        Task ClearAsync();

        #region Cache key
        Key PrepareKey(Key cacheKey, params object[] cacheKeyParameters);
        Key PrepareKeyForDefaultCache(Key cacheKey, params object[] cacheKeyParameters);
        #endregion
    }
}