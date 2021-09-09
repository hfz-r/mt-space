using AHAM.Services.Commission.Dtos.Grpc;
using FluentValidation;

namespace AHAM.Services.Commission.API.Application.Validations
{
    public class CreateRebateCommandValidator : AbstractValidator<FeeRebateDTO>
    {
        public CreateRebateCommandValidator()
        {
            RuleFor(command => command.InvestorId)
                .NotEmpty()
                .WithMessage("Investor ID is required.");
            RuleFor(command => command.Id)
                .NotNull()
                .WithMessage("Id is required.");
            RuleFor(command => command.Coa)
                .NotEmpty()
                .WithMessage("COA is required.");
            RuleFor(command => command.Drcr)
                .MaximumLength(2)
                .WithMessage("DrCr maximum length exceed");
            RuleFor(command => command.SetupDate)
                .NotEmpty()
                .WithMessage("SetupDate is required.");
        }
    }
}