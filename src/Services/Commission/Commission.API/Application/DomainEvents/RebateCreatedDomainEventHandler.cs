using System;
using System.Threading;
using System.Threading.Tasks;
using AHAM.Services.Commission.API.Application.IntegrationEvents;
using AHAM.Services.Commission.API.Application.IntegrationEvents.Events;
using AHAM.Services.Commission.Domain.Events;
using AHAM.Services.Commission.Domain.Exceptions;
using AHAM.Services.Commission.Infrastructure.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using Inv = AHAM.Services.Commission.Domain.AggregatesModel.InvestorAggregate.Investor;

namespace AHAM.Services.Commission.API.Application.DomainEvents
{
    public class RebateCreatedDomainEventHandler : INotificationHandler<RebateCreatedDomainEvent>
    {
        private readonly IUnitOfWork _worker;
        private readonly ICommissionIntegrationEventService _integrationEvent;
        private readonly ILogger<RebateCreatedDomainEventHandler> _logger;

        public RebateCreatedDomainEventHandler(IUnitOfWork worker, ICommissionIntegrationEventService integrationEvent, ILogger<RebateCreatedDomainEventHandler> logger)
        {
            _worker = worker;
            _integrationEvent = integrationEvent;
            _logger = logger;
        }

        public async Task Handle(RebateCreatedDomainEvent rebateEvent, CancellationToken cancellationToken)
        {
            var repository = _worker.GetRepositoryAsync<Inv>();

            try
            {
                var investor = await repository.SingleAsync(i => i.InvestorId == rebateEvent.InvestorId, cancellationToken: cancellationToken);
                if (investor == null) throw new DomainException("Investor not exists in the domain.");

                var rebate = rebateEvent.Rebate;
                rebate.SetInvestorId(investor.InvestorId);

                await _worker.SaveEntitiesAsync(cancellationToken);

                //@integration-event
                var rebateCreated = new RebateCreatedIntegrationEvent();
                await _integrationEvent.AddAndSaveEventAsync(rebateCreated);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}