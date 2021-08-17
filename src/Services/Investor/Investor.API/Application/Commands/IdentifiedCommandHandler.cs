using System;
using System.Threading;
using System.Threading.Tasks;
using AHAM.BuildingBlocks.EventBus.Extensions;
using AHAM.Services.Investor.Infrastructure.Idempotent;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AHAM.Services.Investor.API.Application.Commands
{
    public class IdentifiedCommandHandler<T, R> : IRequestHandler<IdentifiedCommand<T, R>, R> where T : IRequest<R>
    {
        private readonly IMediator _mediator;
        private readonly IRequestManager _requestManager;
        private readonly ILogger<IdentifiedCommandHandler<T, R>> _logger;

        public IdentifiedCommandHandler(IMediator mediator, IRequestManager requestManager, ILogger<IdentifiedCommandHandler<T, R>> logger)
        {
            _mediator = mediator;
            _requestManager = requestManager;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected virtual R CreateResultForDuplicateRequest()
        {
            return default;
        }

        public async Task<R> Handle(IdentifiedCommand<T, R> message, CancellationToken cancellationToken)
        {
            var exists = await _requestManager.ExistAsync(message.Id);
            if (exists) return CreateResultForDuplicateRequest();

            await _requestManager.CreateRequestForCommandAsync<T>(message.Id);
            try
            {
                var command = message.Command;
                var commandName = command.GetGenericTypeName();

                _logger.LogInformation("----- Sending command: {CommandName} - ({@Command})", commandName, command);

                // Send to the actual Handler 
                var result = await _mediator.Send(command, cancellationToken);

                _logger.LogInformation("----- Command result: {@Result} - {CommandName} - ({@Command})", result, commandName, command);

                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}