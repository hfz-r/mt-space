using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using AHAM.BuildingBlocks.RedisCache.Utils;
using Microsoft.Extensions.Configuration;

namespace AHAM.BuildingBlocks.RedisCache.CacheKey
{
    public abstract class CacheKeyService
    {
        private string HashAlgorithm => "SHA1";
        
        protected readonly IConfiguration Configuration;

        protected CacheKeyService(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        #region Protected methods

        protected string PrepareKeyPrefix(string prefix, params object[] prefixParameters)
        {
            return prefixParameters?.Any() ?? false
                ? string.Format(prefix, prefixParameters.Select(CreateCacheKeyParameters).ToArray())
                : prefix;
        }

        protected string CreateIdsHash(IEnumerable<int> ids)
        {
            var identifiers = ids.ToList();

            if (!identifiers.Any())
                return string.Empty;

            var identifiersString = string.Join(", ", identifiers.OrderBy(id => id));
            return HashGenerator.CreateHash(Encoding.UTF8.GetBytes(identifiersString), HashAlgorithm);
        }

        protected object CreateCacheKeyParameters(object parameter)
        {
            return parameter switch
            {
                null => "null",
                IEnumerable<int> ids => CreateIdsHash(ids),
                int id => id,
                decimal param => param.ToString(CultureInfo.InvariantCulture),
                _ => parameter
            };
        }

        #endregion

        public CacheKey PrepareKey(CacheKey cacheKey, params object[] cacheKeyParameters)
        {
            return cacheKey.Create(CreateCacheKeyParameters, cacheKeyParameters);
        }

        public CacheKey PrepareKeyForDefaultCache(CacheKey cacheKey, params object[] cacheKeyParameters)
        {
            var key = cacheKey.Create(CreateCacheKeyParameters, cacheKeyParameters);
            key.CacheTime = Convert.ToInt32(Configuration.GetSection("Redis")["DefaultCacheTime"]);

            return key;
        }
    }
}