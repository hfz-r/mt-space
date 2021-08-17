using System;
using System.Collections.Generic;
using System.Linq;

namespace AHAM.BuildingBlocks.RedisCache.CacheKey
{
    public class CacheKey
    {
        public CacheKey(string key, params string[] prefixes)
        {
            Key = key;
            Prefixes.AddRange(prefixes.Where(prefix => !string.IsNullOrEmpty(prefix)));
        }

        public string Key { get; protected set; }
        public List<string> Prefixes { get; protected set; } = new List<string>();
        public int CacheTime { get; set; } = 60;

        public virtual CacheKey Create(Func<object, object> createCacheKeyParameters, params object[] keyObjects)
        {
            var cacheKey = new CacheKey(Key, Prefixes.ToArray());

            if (!keyObjects.Any()) return cacheKey;

            cacheKey.Key = string.Format(cacheKey.Key, keyObjects.Select(createCacheKeyParameters).ToArray());

            for (var i = 0; i < cacheKey.Prefixes.Count; i++)
                cacheKey.Prefixes[i] = string.Format(cacheKey.Prefixes[i],
                    keyObjects.Select(createCacheKeyParameters).ToArray());

            return cacheKey;
        }
    }
}