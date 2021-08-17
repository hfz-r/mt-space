using System.Linq;
using System.Threading.Tasks;
using AHAM.Services.Investor.Domain.SeedWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AHAM.Services.Investor.Infrastructure
{
    static class MediatorExtension
    {
        public static async Task DispatchDomainEventsAsync<TContext>(this IMediator mediator, TContext ctx)
            where TContext : DbContext
        {
            var domainEntities = ctx.ChangeTracker
                .Entries<Entity>()
                .Where(e => e.Entity.DomainEvents != null && e.Entity.DomainEvents.Any());

            var domainEvents = domainEntities
                .SelectMany(e => e.Entity.DomainEvents)
                .ToList();

            domainEntities.ToList()
                .ForEach(e => e.Entity.ClearDomainEvents());

            foreach (var domainEvent in domainEvents)
                await mediator.Publish(domainEvent);
        }
    }
}