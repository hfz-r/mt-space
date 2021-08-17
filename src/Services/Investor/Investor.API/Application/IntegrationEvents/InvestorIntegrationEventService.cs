using System;
using System.Data.Common;
using System.Threading.Tasks;
using AHAM.BuildingBlocks.EventBus.Abstractions;
using AHAM.BuildingBlocks.EventBus.Events;
using AHAM.BuildingBlocks.IntegrationEventLog.Services;
using AHAM.Services.Investor.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AHAM.Services.Investor.API.Application.IntegrationEvents
{
    public class InvestorIntegrationEventService : IInvestorIntegrationEventService
    {
        private readonly InvestorContext _investorContext;
        private readonly IEventBus _eventBus;
        private readonly IIntegrationEventLogService _eventLogService;
        private readonly ILogger<InvestorIntegrationEventService> _logger;

        public InvestorIntegrationEventService(
            Func<DbConnection, IIntegrationEventLogService> eventLogServiceFactory,
            InvestorContext investorContext,
            IEventBus eventBus, 
            ILogger<InvestorIntegrationEventService> logger)
        {
            var svcFactory = eventLogServiceFactory ?? throw new ArgumentNullException(nameof(eventLogServiceFactory));
            _investorContext = investorContext ?? throw new ArgumentNullException(nameof(investorContext));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            _eventLogService = svcFactory(_investorContext.Database.GetDbConnection());
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task PublishEventsThroughEventBusAsync(Guid transactionId)
        {
            var pendingLogEvents = await _eventLogService.RetrieveEventLogsPendingToPublishAsync(transactionId);

            foreach (var logEvent in pendingLogEvents)
            {
                _logger.LogInformation(
                    "----- Publishing integration event: {IntegrationEventId} from {AppName} - ({@IntegrationEvent})",
                    logEvent.EventId, Program.AppName, logEvent.IntegrationEvent);

                try
                {
                    await _eventLogService.MarkEventAsInProgressAsync(logEvent.EventId);

                    _eventBus.Publish(logEvent.IntegrationEvent);

                    await _eventLogService.MarkEventAsPublishedAsync(logEvent.EventId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "ERROR publishing integration event: {IntegrationEventId} from {AppName}", logEvent.EventId, Program.AppName);

                    await _eventLogService.MarkEventAsFailedAsync(logEvent.EventId);
                }
            }
        }

        public async Task AddAndSaveEventAsync(IntegrationEvent evt)
        {
            _logger.LogInformation("----- En-queuing integration event {IntegrationEventId} to repository ({@IntegrationEvent})", evt.Id, evt);

            await _eventLogService.SaveEventAsync(evt, _investorContext.GetCurrentTransaction());
        }
    }
}