using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AHAM.BuildingBlocks.RedisCache;
using AHAM.BuildingBlocks.RedisCache.CacheKey;
using AHAM.Services.Investor.Domain.SeedWork;
using AHAM.Services.Investor.Infrastructure.Caching;
using AHAM.Services.Investor.Infrastructure.Paging;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace AHAM.Services.Investor.Infrastructure.Repositories
{
    public class RepositoryAsync<T> : IRepositoryAsync<T> where T : Entity, IAggregateRoot
    {
        private readonly DbContext _dbContext;
        private readonly DbSet<T> _dbSet;
        private readonly ICacheManager _cache;

        public RepositoryAsync(DbContext dbContext, ICacheManager cache)
        {
            _dbContext = dbContext;
            _cache = cache;
            _dbSet = _dbContext.Set<T>();
        }

        #region Private methods

        private TReturn FromCache<TReturn>(Func<TReturn> callback, Func<ICacheManager, CacheKey> cacheKey)
        {
            if (cacheKey == null) return callback();

            var key = cacheKey(_cache) ?? _cache.PrepareKeyForDefaultCache(Keys<T>.DefaultCacheKey);
            return _cache.Get(key, callback);
        }

        private async Task<TReturn> FromCacheAsync<TReturn>(Func<Task<TReturn>> callback, Func<ICacheManager, CacheKey> cacheKey)
        {
            if (cacheKey == null) return await callback();

            var key = cacheKey(_cache) ?? _cache.PrepareKeyForDefaultCache(Keys<T>.DefaultCacheKey);
            return await _cache.GetAsync(key, callback);
        }

        #endregion

        public async Task<T> SingleAsync(
            Expression<Func<T, bool>> predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null,
            Func<ICacheManager, CacheKey> cacheKey = null,
            bool disableTracking = false,
            CancellationToken cancellationToken = default)
        {
            async Task<T> FetchAsync()
            {
                IQueryable<T> query = _dbSet;

                if (disableTracking) query = query.AsNoTracking();

                if (include != null) query = include(query);

                if (predicate != null) query = query.Where(predicate);

                return orderBy != null
                    ? await orderBy(query).FirstOrDefaultAsync(cancellationToken)
                    : await query.FirstOrDefaultAsync(cancellationToken);
            }

            return await FromCacheAsync(FetchAsync, cacheKey);
        }
        
        public async Task<IList<T>> GetListAsync(
            Expression<Func<T, bool>> predicate = null,
            Func<IQueryable<T>, IQueryable<T>> queryExp = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null,
            Func<ICacheManager, CacheKey> cacheKey = null,
            bool disableTracking = false,
            CancellationToken cancellationToken = default)
        {
            async Task<IList<T>> FetchAsync()
            {
                IQueryable<T> query = _dbSet;

                if (disableTracking) query = query.AsNoTracking();

                if (include != null) query = include(query);

                if (queryExp != null) query = queryExp(query);

                if (predicate != null) query = query.Where(predicate);

                return orderBy != null
                    ? await orderBy(query).ToListAsync(cancellationToken)
                    : await query.ToListAsync(cancellationToken);
            }

            return await FromCacheAsync(FetchAsync, cacheKey);
        }

        public async Task<Paginate<T>> GetPagedListAsync(
            Expression<Func<T, bool>> predicate = null,
            Func<IQueryable<T>, IQueryable<T>> queryExp = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null,
            Func<ICacheManager, CacheKey> cacheKey = null,
            int index = 0,
            int size = int.MaxValue,
            int from = 0,
            bool disableTracking = false,
            CancellationToken cancellationToken = default)
        {
            async Task<Paginate<T>> FetchAsync()
            {
                IQueryable<T> query = _dbSet;

                if (disableTracking) query = query.AsNoTracking();

                if (include != null) query = include(query);

                if (queryExp != null) query = queryExp(query);

                if (predicate != null) query = query.Where(predicate);

                return new Paginate<T>(
                    orderBy != null
                        ? await orderBy(query).ToListAsync(cancellationToken)
                        : await query.ToListAsync(cancellationToken), index, size, from);
            }

            return await FromCacheAsync(FetchAsync, cacheKey);
        }

        public async Task<T> FindAsync(params object[] keys)
        {
            return await _dbSet.FindAsync(keys);
        }

        public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            await _dbSet.AddAsync(entity, cancellationToken);
        }

        public async Task AddAsync(params T[] entities)
        {
            await _dbSet.AddRangeAsync(entities);
        }

        public async Task AddAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
        {
            await _dbSet.AddRangeAsync(entities, cancellationToken);
        }

        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        public void Delete(params T[] entities)
        {
            _dbSet.RemoveRange(entities);
        }

        public void Delete(IEnumerable<T> entities)
        {
            _dbSet.RemoveRange(entities);
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        public void Update(params T[] entities)
        {
            _dbSet.UpdateRange(entities);
        }

        public void Update(IEnumerable<T> entities)
        {
            _dbSet.UpdateRange(entities);
        }

        public void Dispose()
        {
            if (!(_dbContext is { } dbContext)) throw new InvalidOperationException("Context does not support operation");

            dbContext.Dispose();
        }

        #region Cache helper methods

        public async Task RemoveByPrefixAsync(string prefix, params object[] prefixParameters)
        {
            await _cache.RemoveByPrefixAsync(prefix, prefixParameters);
        }

        #endregion
    }
}