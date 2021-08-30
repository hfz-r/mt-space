using AHAM.Services.Investor.API.Application.Commands;
using FluentValidation;

namespace AHAM.Services.Investor.API.Application.Validations
{
    public class CreateRebateCommandValidator : AbstractValidator<CreateRebateCommand>
    {
        public CreateRebateCommandValidator()
        {
            RuleFor(command => command.InvestorId)
                .NotEmpty()
                .WithMessage("Investor ID is required.");
            RuleForEach(command => command.List)
                .ChildRules(list =>
                {
                    list.RuleFor(child => child.Coa)
                        .NotEmpty()
                        .WithMessage("COA is required.");
                    list.RuleFor(child => child.Drcr)
                        .MaximumLength(2)
                        .WithMessage("DrCr maximum length exceed");
                    list.RuleFor(child => child.SetupDate)
                        .NotEmpty()
                        .WithMessage("SetupDate is required.");
                });
        }
    }
}