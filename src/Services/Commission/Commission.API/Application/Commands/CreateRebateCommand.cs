using System.Collections.Generic;
using AHAM.Services.Commission.API.Application.Validations;
using AHAM.Services.Commission.Dtos.Grpc;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AHAM.Services.Commission.API.Application.Commands
{
    public class CreateRebateCommand : IRequest<bool>
    {
        public IList<FeeRebateDTO> List { get; set; }
    }

    public class RebateCommandValidator : AbstractValidator<CreateRebateCommand>
    {
        public RebateCommandValidator(ILogger<RebateCommandValidator> logger)
        {
            RuleForEach(x => x.List).SetValidator(new CreateRebateCommandValidator());

            logger.LogTrace("----- INSTANCE CREATED - {ClassName}", GetType().Name);
        }
    }
}