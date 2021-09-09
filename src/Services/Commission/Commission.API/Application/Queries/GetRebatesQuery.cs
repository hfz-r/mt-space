using AHAM.Services.Commission.API.Application.Validations;
using AHAM.Services.Commission.Domain.AggregatesModel.FeeRebateAggregate;
using AHAM.Services.Commission.Infrastructure.Paging;
using AHAM.Services.Commission.Investor.Grpc;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AHAM.Services.Commission.API.Application.Queries
{
    public class GetRebatesQuery : IRequest<IPaginate<FeeRebate>>
    {
        public GetRebatesRequest Request { get; set; }
    }

    public class RebateQueryValidator : AbstractValidator<GetRebatesQuery>
    {
        public RebateQueryValidator(ILogger<RebateQueryValidator> logger)
        {
            RuleFor(x => x.Request).SetValidator(new GetRebatesQueryValidator());

            logger.LogTrace("----- INSTANCE CREATED - {ClassName}", GetType().Name);
        }
    }
}