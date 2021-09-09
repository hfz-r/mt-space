using System;
using System.Threading.Tasks;

namespace AHAM.Services.Commission.Infrastructure.Idempotent
{
    public interface IRequestManager
    {
        Task<bool> ExistAsync(Guid id);
        Task CreateRequestForCommandAsync<T>(Guid id);
    }
}