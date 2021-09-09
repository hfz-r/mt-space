using System;
using System.Threading;
using System.Threading.Tasks;
using AHAM.Services.Commission.Domain.SeedWork;

namespace AHAM.Services.Commission.Infrastructure.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IRepositoryAsync<TEntity> GetRepositoryAsync<TEntity>() where TEntity : Entity, IAggregateRoot;
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default);
    }

    public interface IUnitOfWork<out TContext> : IUnitOfWork
    {
        TContext Context { get; }
    }
}