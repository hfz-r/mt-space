using AHAM.Services.Commission.API.Application.Queries;
using FluentValidation;

namespace AHAM.Services.Commission.API.Application.Validations
{
    public class GetRebatesQueryValidator : AbstractValidator<GetRebatesQuery>
    {
        public GetRebatesQueryValidator()
        {
            //todo: (future use - prop validation)
            //RuleFor(query => query.InvestorId).NotEmpty();
            //RuleFor(query => query.Coa).NotEmpty();
        }
    }
}