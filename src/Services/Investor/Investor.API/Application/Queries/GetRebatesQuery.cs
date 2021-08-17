using AHAM.Services.Investor.API.Application.Validations;
using AHAM.Services.Investor.Domain.AggregatesModel.FeeRebateAggregate;
using AHAM.Services.Investor.Grpc;
using AHAM.Services.Investor.Infrastructure.Paging;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AHAM.Services.Investor.API.Application.Queries
{
    public class GetRebatesQuery : IRequest<IPaginate<FeeRebate>>
    {
        public GetRebatesRequest Request { get; set; }
    }

    public class QueryValidator : AbstractValidator<GetRebatesQuery>
    {
        public QueryValidator(ILogger<QueryValidator> logger)
        {
            RuleFor(x => x.Request).SetValidator(new GetRebatesQueryValidator());

            logger.LogTrace("----- INSTANCE CREATED - {ClassName}", GetType().Name);
        }
    }
}