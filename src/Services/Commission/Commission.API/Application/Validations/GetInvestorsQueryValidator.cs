using AHAM.Services.Commission.Investor.Grpc;
using FluentValidation;

namespace AHAM.Services.Commission.API.Application.Validations
{
    public class GetInvestorsQueryValidator : AbstractValidator<GetInvestorsRequest>
    {
        public GetInvestorsQueryValidator()
        {
            //todo: (future use - prop validation)
        }
    }
}