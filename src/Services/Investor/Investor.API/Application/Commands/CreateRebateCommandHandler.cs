using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AHAM.Services.Investor.API.Application.IntegrationEvents;
using AHAM.Services.Investor.Domain.AggregatesModel.FeeRebateAggregate;
using AHAM.Services.Investor.Infrastructure.Idempotent;
using AHAM.Services.Investor.Infrastructure.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AHAM.Services.Investor.API.Application.Commands
{
    public class CreateRebateCommandHandler : IRequestHandler<CreateRebateCommand, bool>
    {
        private readonly IUnitOfWork _worker;
        private readonly IInvestorIntegrationEventService _integrationEvent;
        private readonly ILogger<CreateRebateCommandHandler> _logger;

        public CreateRebateCommandHandler(IUnitOfWork worker, IInvestorIntegrationEventService integrationEvent, ILogger<CreateRebateCommandHandler> logger)
        {
            _worker = worker;
            _integrationEvent = integrationEvent;
            _logger = logger;
        }

        public async Task<bool> Handle(CreateRebateCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var rebates = command.Request.Rebates;

                var src = rebates.Select(r =>
                    new FeeRebate(r.Amc, r.Agent, r.Channel, r.Coa, r.Drcr, command.Request.InvestorId, r.Plan,
                        r.SetupBy, r.SetupType, r.Type, r.Currency)
                );

                _logger.LogInformation("----- Creating Order - Order: {@Order}", "");

                var repository = _worker.GetRepositoryAsync<FeeRebate>();
                await repository.AddAsync(src, cancellationToken);

                //publish @domain-event
                return await _worker.SaveEntitiesAsync(cancellationToken);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }

    public class CreateOrderIdentifiedCommandHandler : IdentifiedCommandHandler<CreateRebateCommand, bool>
    {
        public CreateOrderIdentifiedCommandHandler(
            IMediator mediator,
            IRequestManager requestManager,
            ILogger<IdentifiedCommandHandler<CreateRebateCommand, bool>> logger) : base(mediator, requestManager, logger)
        {
        }

        protected override bool CreateResultForDuplicateRequest() => true;
    }
}