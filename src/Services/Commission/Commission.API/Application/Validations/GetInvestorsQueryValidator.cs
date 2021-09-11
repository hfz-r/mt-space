using AHAM.Services.Commission.API.Application.Queries;
using FluentValidation;

namespace AHAM.Services.Commission.API.Application.Validations
{
    public class GetInvestorsQueryValidator : AbstractValidator<GetInvestorsQuery>
    {
        public GetInvestorsQueryValidator()
        {
            //todo: (future use - prop validation)
        }
    }
}