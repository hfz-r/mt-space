using System;
using System.Threading;
using System.Threading.Tasks;
using AHAM.Services.Investor.API.Application.IntegrationEvents;
using AHAM.Services.Investor.API.Application.IntegrationEvents.Events;
using AHAM.Services.Investor.Domain.AggregatesModel.FeeRebateAggregate;
using AHAM.Services.Investor.Domain.Events;
using AHAM.Services.Investor.Domain.Exceptions;
using AHAM.Services.Investor.Infrastructure.Caching;
using AHAM.Services.Investor.Infrastructure.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using Inv = AHAM.Services.Investor.Domain.AggregatesModel.InvestorAggregate.Investor;

namespace AHAM.Services.Investor.API.Application.DomainEvents
{
    public class RebateCreatedDomainEventHandler : INotificationHandler<RebateCreatedDomainEvent>
    {
        private readonly IUnitOfWork _worker;
        private readonly IInvestorIntegrationEventService _integrationEvent;
        private readonly ILogger<RebateCreatedDomainEventHandler> _logger;

        public RebateCreatedDomainEventHandler(IUnitOfWork worker, IInvestorIntegrationEventService integrationEvent, ILogger<RebateCreatedDomainEventHandler> logger)
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

                _logger.LogInformation("----- Finalizing Rebate - FeeRebate: {@Rebate}", rebate);

                await _worker.SaveEntitiesAsync(cancellationToken);
                await repository.RemoveByPrefixAsync(Keys<FeeRebate>.RebatesPrefix);

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