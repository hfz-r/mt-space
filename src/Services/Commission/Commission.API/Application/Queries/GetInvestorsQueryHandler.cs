using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AHAM.Services.Commission.Infrastructure.Caching;
using AHAM.Services.Commission.Infrastructure.Paging;
using AHAM.Services.Commission.Infrastructure.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using Inv = AHAM.Services.Commission.Domain.AggregatesModel.InvestorAggregate.Investor;

namespace AHAM.Services.Commission.API.Application.Queries
{
    public class GetInvestorsQueryHandler : IRequestHandler<GetInvestorsQuery, IPaginate<Inv>>
    {
        private readonly IUnitOfWork _worker;
        private readonly ILogger<GetInvestorsQueryHandler> _logger;

        public GetInvestorsQueryHandler(IUnitOfWork worker, ILogger<GetInvestorsQueryHandler> logger)
        {
            _worker = worker;
            _logger = logger;
        }

        public async Task<IPaginate<Inv>> Handle(GetInvestorsQuery query, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Fetching investor queries from database. Handler name: {HandlerName}", nameof(GetInvestorsQueryHandler));

                var repository = _worker.GetRepositoryAsync<Inv>();
                var investors = await repository.GetPagedListAsync(
                    queryExp: q =>
                    {
                        if (!string.IsNullOrEmpty(query.InvestorId))
                            q = q.Where(i => i.InvestorId == query.InvestorId);
                        if (!string.IsNullOrEmpty(query.InvestorName))
                            q = q.Where(i => i.InvestorName == query.InvestorName);
                        return q;
                    },
                    orderBy: q => q.OrderBy(i => i.InvestorName),
                    cacheKey: cache => cache.PrepareKeyForDefaultCache(Keys<Inv>.ListCacheKey, query.Size, query.InvestorId, query.InvestorName),
                    disableTracking: true,
                    index: query.Index,
                    size: query.Size,
                    from: query.From,
                    cancellationToken: cancellationToken
                );

                return investors;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}