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
                        if (!string.IsNullOrEmpty(query.Request.InvestorId))
                            q = q?.Where(i => i.InvestorId == query.Request.InvestorId);
                        if (!string.IsNullOrEmpty(query.Request.InvestorName))
                            q = q?.Where(i => i.InvestorName == query.Request.InvestorName);
                        return q;
                    },
                    orderBy: q => q.OrderBy(i => i.InvestorId),
                    cacheKey: cache => cache.PrepareKeyForDefaultCache(Keys<Inv>.ListCacheKey, query.Request.Size, query.Request.InvestorId, query.Request.InvestorName),
                    disableTracking: true,
                    index: query.Request.Index,
                    size: query.Request.Size,
                    from: query.Request.From,
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