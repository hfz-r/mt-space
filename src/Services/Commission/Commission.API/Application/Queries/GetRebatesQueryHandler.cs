using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AHAM.Services.Commission.Domain.AggregatesModel.FeeRebateAggregate;
using AHAM.Services.Commission.Infrastructure.Caching;
using AHAM.Services.Commission.Infrastructure.Paging;
using AHAM.Services.Commission.Infrastructure.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AHAM.Services.Commission.API.Application.Queries
{
    public class GetRebatesQueryHandler : IRequestHandler<GetRebatesQuery, IPaginate<FeeRebate>>
    {
        private readonly IUnitOfWork _worker;
        private readonly ILogger<GetRebatesQueryHandler> _logger;

        public GetRebatesQueryHandler(IUnitOfWork worker, ILogger<GetRebatesQueryHandler> logger)
        {
            _worker = worker;
            _logger = logger;
        }

        public async Task<IPaginate<FeeRebate>> Handle(GetRebatesQuery query, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Fetching rebate queries from database. Handler name: {HandlerName}", nameof(GetRebatesQueryHandler));

                var repository = _worker.GetRepositoryAsync<FeeRebate>();
                var rebates = await repository.GetPagedListAsync(
                    queryExp: q =>
                    {
                        if (!string.IsNullOrEmpty(query.Request.Coa)) q = q?.Where(r => r.Coa == query.Request.Coa);
                        return q;
                    },
                    clientEval: q =>
                    {
                        if (!string.IsNullOrEmpty(query.Request.InvestorId)) 
                            q = q
                                .Where(r => r.InvestorId == query.Request.InvestorId)
                                .OrderBy(r => r.InvestorId);
                        return q;
                    },
                    orderBy: q => q .OrderByDescending(r => r.SetupDate),
                    cacheKey: cache => cache.PrepareKeyForDefaultCache(Keys<FeeRebate>.ListCacheKey, query.Request.Size, query.Request.InvestorId, query.Request.Coa),
                    disableTracking: true,
                    index: query.Request.Index,
                    size: query.Request.Size,
                    from: query.Request.From,
                    cancellationToken: cancellationToken
                );

                return rebates;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}