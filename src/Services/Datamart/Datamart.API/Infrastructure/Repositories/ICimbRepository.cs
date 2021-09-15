using System.Collections.Generic;
using System.Threading.Tasks;
using AHAM.Services.Datamart.API.Model;
using AHAM.Services.Datamart.API.Model.Envelopes;

namespace AHAM.Services.Datamart.API.Infrastructure.Repositories
{
    public interface ICimbRepository
    {
        Task<IList<FeeStructure>> GetAsync(CimbQuery query);
        Task UpsertAsync(IEnumerable<CimbCommand> commands);
    }
}