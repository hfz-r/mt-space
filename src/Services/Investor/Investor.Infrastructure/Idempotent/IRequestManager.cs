using System;
using System.Threading.Tasks;

namespace AHAM.Services.Investor.Infrastructure.Idempotent
{
    public interface IRequestManager
    {
        Task<bool> ExistAsync(Guid id);
        Task CreateRequestForCommandAsync<T>(Guid id);
    }
}