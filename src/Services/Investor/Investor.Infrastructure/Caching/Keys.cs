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

        public static CacheKey RebatesCacheKey => new CacheKey("AHAM.Rebates.{0}-{1}", RebatesPrefix, Prefix);
        public static CacheKey InvestorCacheKey => new CacheKey("AHAM.Investor.{0}", RebatesPrefix, Prefix);
        public static string RebatesPrefix => "AHAM.Rebates.";

        public static string Prefix => $"AHAM.{EntityName}.";
    }
}