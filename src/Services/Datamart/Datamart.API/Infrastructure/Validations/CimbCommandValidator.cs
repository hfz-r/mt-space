using AHAM.Services.Datamart.CimbSplit.Grpc;
using FluentValidation;

namespace AHAM.Services.Datamart.API.Infrastructure.Validations
{
    public class CimbCommandValidator : AbstractValidator<CreateCimbRequest>
    {
        public CimbCommandValidator()
        {
            RuleForEach(p => p.Cimb)
                .ChildRules(p =>
                {
                    p.RuleFor(c => c.Id)
                        .NotEmpty()
                        .WithMessage("Id is required.");
                    p.RuleFor(c => c.ProductCode)
                        .NotEmpty()
                        .WithMessage("ProductCode is required.");
                });
        }
    }
}