using System;
using System.Threading.Tasks;
using AHAM.Services.Investor.Domain.Exceptions;

namespace AHAM.Services.Investor.Infrastructure.Idempotent
{
    public class RequestManager : IRequestManager
    {
        private readonly InvestorContext _context;

        public RequestManager(InvestorContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<bool> ExistAsync(Guid id)
        {
            var request = await _context.FindAsync<ClientRequest>(id);

            return request != null;
        }

        public async Task CreateRequestForCommandAsync<T>(Guid id)
        {
            var exists = await ExistAsync(id);

            var request = exists
                ? throw new DomainException($"Request with {id} already exists")
                : new ClientRequest
                {
                    Id = id,
                    Name = typeof(T).Name,
                    Time = DateTime.UtcNow
                };

            _context.Add(request);

            await _context.SaveChangesAsync();
        }
    }
}