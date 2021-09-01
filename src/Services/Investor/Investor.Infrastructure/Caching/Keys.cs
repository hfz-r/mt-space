using AHAM.BuildingBlocks.RedisCache.CacheKey;
using AHAM.Services.Investor.Domain.SeedWork;

namespace AHAM.Services.Investor.Infrastructure.Caching
{
    public static class Keys<TEntity> where TEntity : Entity
    {
        public static string EntityName => typeof(TEntity).Name;

        public static CacheKey DefaultCacheKey => new CacheKey($"AHAM.{EntityName}.default.", DefaultPrefix, Prefix);
        public static string DefaultPrefix => $"AHAM.{EntityName}.default.";

        public static CacheKey ByIdCacheKey => new CacheKey($"AHAM.{EntityName}.by-id.{{0}}", ByIdPrefix, Prefix);
        public static string ByIdPrefix => $"AHAM.{EntityName}.by-id.";

        public static CacheKey ListCacheKey => new CacheKey($"AHAM.{EntityName}.list.{{0}}-{{1}}", ListPrefix, Prefix);
        public static string ListPrefix => $"AHAM.{EntityName}.list";

        //universal prefix
        public static string Prefix => $"AHAM.{EntityName}.";
    }
}