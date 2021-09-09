using AHAM.Services.Commission.API.Application.Validations;
using AHAM.Services.Commission.Infrastructure.Paging;
using AHAM.Services.Commission.Investor.Grpc;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Inv = AHAM.Services.Commission.Domain.AggregatesModel.InvestorAggregate.Investor;

namespace AHAM.Services.Commission.API.Application.Queries
{
    public class GetInvestorsQuery : IRequest<IPaginate<Inv>>
    {
        public GetInvestorsRequest Request { get; set; }
    }

    public class InvestorQueryValidator : AbstractValidator<GetInvestorsQuery>
    {
        public InvestorQueryValidator(ILogger<InvestorQueryValidator> logger)
        {
            RuleFor(x => x.Request).SetValidator(new GetInvestorsQueryValidator());

            logger.LogTrace("----- INSTANCE CREATED - {ClassName}", GetType().Name);
        }
    }
}