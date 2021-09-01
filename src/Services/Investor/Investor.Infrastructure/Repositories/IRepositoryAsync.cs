using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AHAM.BuildingBlocks.RedisCache;
using AHAM.BuildingBlocks.RedisCache.CacheKey;
using AHAM.Services.Investor.Domain.SeedWork;
using AHAM.Services.Investor.Infrastructure.Paging;
using Microsoft.EntityFrameworkCore.Query;

namespace AHAM.Services.Investor.Infrastructure.Repositories
{
    public interface IRepositoryAsync<T> where T : Entity, IAggregateRoot
    {
        Task<T> SingleAsync(
            Expression<Func<T, bool>> predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null,
            Func<ICacheManager, CacheKey> cacheKey = null,
            bool disableTracking = false,
            CancellationToken cancellationToken = default);

        Task<IList<T>> GetListAsync(
            Expression<Func<T, bool>> predicate = null,
            Func<IQueryable<T>, IQueryable<T>> queryExp = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null,
            Func<ICacheManager, CacheKey> cacheKey = null,
            bool disableTracking = false,
            CancellationToken cancellationToken = default);

        Task<IPaginate<T>> GetPagedListAsync(
            Expression<Func<T, bool>> predicate = null,
            Func<IQueryable<T>, IQueryable<T>> queryExp = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null,
            Func<ICacheManager, CacheKey> cacheKey = null,
            int index = 0,
            int size = int.MaxValue,
            int from = 0,
            bool disableTracking = false,
            CancellationToken cancellationToken = default);

        Task<T> FindAsync(params object[] keys);
        Task AddAsync(T entity, CancellationToken cancellationToken = default);
        Task AddAsync(params T[] entities);
        Task AddAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);
        void Delete(T entity);
        void Delete(params T[] entities);
        void Delete(IEnumerable<T> entities);
        void Update(T entity);
        void Update(params T[] entities);
        void Update(IEnumerable<T> entities);
        void Dispose();

        #region Cache helper methods

        Task RemoveByPrefixAsync(string prefix, params object[] prefixParameters);

        #endregion
    }
}