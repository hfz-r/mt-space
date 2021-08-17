using AHAM.Services.Investor.API.Application.Validations;
using AHAM.Services.Investor.Grpc;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AHAM.Services.Investor.API.Application.Commands
{
    public class CreateRebateCommand : IRequest<bool>
    {
        public CreateRebateRequest Request { get; set; }
    }

    public class CommandValidator : AbstractValidator<CreateRebateCommand>
    {
        public CommandValidator(ILogger<CommandValidator> logger)
        {
            RuleFor(x => x.Request).SetValidator(new CreateRebateCommandValidator());

            logger.LogTrace("----- INSTANCE CREATED - {ClassName}", GetType().Name);
        }
    }
}