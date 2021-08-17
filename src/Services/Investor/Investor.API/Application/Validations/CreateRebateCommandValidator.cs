using AHAM.Services.Investor.Grpc;
using FluentValidation;

namespace AHAM.Services.Investor.API.Application.Validations
{
    public class CreateRebateCommandValidator : AbstractValidator<CreateRebateRequest>
    {
        public CreateRebateCommandValidator()
        {
            RuleFor(command => command.InvestorId).NotEmpty();
            RuleForEach(command => command.Rebates)
                .ChildRules(orders =>
                {
                    orders.RuleFor(x => x.Amc).NotEmpty();
                    orders.RuleFor(x => x.Coa).NotEmpty();
                    orders.RuleFor(x => x.Currency).NotEmpty();
                    orders.RuleFor(x => x.Drcr).NotEmpty();
                    orders.RuleFor(x => x.SetupBy).NotEmpty();
                    orders.RuleFor(x => x.SetupBy).NotEmpty();
                    orders.RuleFor(x => x.Type).NotEmpty();
                });
        }
    }
}