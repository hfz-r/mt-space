using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AHAM.BuildingBlocks.RedisCache.CacheKey;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Key = AHAM.BuildingBlocks.RedisCache.CacheKey.CacheKey;

namespace AHAM.BuildingBlocks.RedisCache
{
    public class CacheManager : CacheKeyService, ICacheManager
    {
        private readonly IDistributedCache _distributedCache;
        private static readonly IList<string> Keys;
        private static readonly SemaphoreSlim SemaphoreSlim;

        static CacheManager()
        {
            Keys = new List<string>();
            SemaphoreSlim = new SemaphoreSlim(1, 1);
        }

        public CacheManager(IConfiguration configuration, IDistributedCache distributedCache) : base(configuration)
        {
            _distributedCache = distributedCache;
        }

        #region Private methods

        private DistributedCacheEntryOptions PrepareEntryOptions(Key key)
        {
            //set expiration time for the passed cache key
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(key.CacheTime)
            };

            return options;
        }

        private (bool isSet, T item) TryGetItem<T>(Key key)
        {
            var json = _distributedCache.GetString(key.Key);

            if (string.IsNullOrEmpty(json)) return (false, default);

            var item = JsonConvert.DeserializeObject<T>(json);

            return (true, item);
        }

        private async Task<(bool isSet, T item)> TryGetItemAsync<T>(Key cache)
        {
            var json = await _distributedCache.GetStringAsync(cache.Key);

            if (string.IsNullOrEmpty(json)) return (false, default);

            var item = JsonConvert.DeserializeObject<T>(json);

            return (true, item);
        }

        #endregion

        public T Get<T>(Key key, Func<T> acquire)
        {
            if (key.CacheTime <= 0) return acquire();

            var (isSet, item) = TryGetItem<T>(key);
            if (isSet) return item;

            var result = acquire();

            if (result != null) Set(key, result);

            return result;
        }

        public async Task<T> GetAsync<T>(Key key, Func<Task<T>> acquire)
        {
            if (key.CacheTime <= 0) return await acquire();

            var (isSet, item) = await TryGetItemAsync<T>(key);
            if (isSet) return item;

            var result = await acquire();
            if (result != null) await SetAsync(key, result);

            return result;
        }

        public async Task<T> GetAsync<T>(Key cache, Func<T> acquire)
        {
            if (cache.CacheTime <= 0) return acquire();

            var (isSet, item) = await TryGetItemAsync<T>(cache);
            if (isSet) return item;

            var result = acquire();
            if (result != null) await SetAsync(cache, result);

            return result;
        }

        public void Set(Key cache, object data)
        {
            try
            {
                if ((cache?.CacheTime ?? 0) <= 0 || data == null) return;

                _distributedCache.SetString(cache.Key, JsonConvert.SerializeObject(data), PrepareEntryOptions(cache));
                SemaphoreSlim.Wait();

                Keys.Add(cache.Key);
            }
            catch (Exception ex)
            {
                throw new Exception("Cache failed.", ex);
            }
            finally
            {
                SemaphoreSlim.Release();
            }
        }

        public async Task SetAsync(Key cache, object data)
        {
            try
            {
                if ((cache?.CacheTime ?? 0) <= 0 || data == null) return;

                await _distributedCache.SetStringAsync(cache.Key, JsonConvert.SerializeObject(data), PrepareEntryOptions(cache));
                await SemaphoreSlim.WaitAsync();

                Keys.Add(cache.Key);
            }
            catch (Exception ex)
            {
                throw new Exception("Cache failed.", ex);
            }
            finally
            {
                SemaphoreSlim.Release();
            }
        }

        public async Task RemoveAsync(Key cache, params object[] cacheKeyParameters)
        {
            try
            {
                cache = PrepareKey(cache, cacheKeyParameters);

                await _distributedCache.RemoveAsync(cache.Key);
                await SemaphoreSlim.WaitAsync();

                Keys.Remove(cache.Key);
            }
            catch (Exception ex)
            {
                throw new Exception("Cache failed.", ex);
            }
            finally
            {
                SemaphoreSlim.Release();
            }
        }

        /// <summary>
        /// Remove items by cache key prefix
        /// </summary>
        /// <param name="prefix">Cache key prefix</param>
        /// <param name="prefixParameters">Parameters to create cache key prefix</param>
        public async Task RemoveByPrefixAsync(string prefix, params object[] prefixParameters)
        {
            try
            {
                prefix = PrepareKeyPrefix(prefix, prefixParameters);

                await SemaphoreSlim.WaitAsync();

                foreach (var key in Keys
                    .Where(key => key.StartsWith(prefix, StringComparison.InvariantCultureIgnoreCase)).ToList())
                {
                    await _distributedCache.RemoveAsync(key);
                    Keys.Remove(key);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Cache failed.", ex);
            }
            finally
            {
                SemaphoreSlim.Release();
            }
        }

        /// <summary>
        /// Clear all cache data
        /// </summary>
        public async Task ClearAsync()
        {
            try
            {
                await SemaphoreSlim.WaitAsync();

                foreach (var key in Keys) await _distributedCache.RemoveAsync(key);

                Keys.Clear();
            }
            catch (Exception ex)
            {
                throw new Exception("Cache failed.", ex);
            }
            finally
            {
                SemaphoreSlim.Release();
            }
        }

        /// <summary>
        /// Free, release or reset unmanaged resources on application-centric 
        /// </summary>
        public void Dispose()
        {
        }
    }
}