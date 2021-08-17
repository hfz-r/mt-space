using AHAM.Services.Investor.Grpc;
using FluentValidation;

namespace AHAM.Services.Investor.API.Application.Validations
{
    public class GetRebatesQueryValidator : AbstractValidator<GetRebatesRequest>
    {
        public GetRebatesQueryValidator()
        {
            //todo: (future use - prop validation)
            //RuleFor(query => query.InvestorId).NotEmpty();
            //RuleFor(query => query.Coa).NotEmpty();
        }
    }
}