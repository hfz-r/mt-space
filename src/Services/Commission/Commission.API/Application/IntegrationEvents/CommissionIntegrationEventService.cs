using System;
using System.Data.Common;
using System.Threading.Tasks;
using AHAM.BuildingBlocks.EventBus.Abstractions;
using AHAM.BuildingBlocks.EventBus.Events;
using AHAM.BuildingBlocks.IntegrationEventLog.Services;
using AHAM.Services.Commission.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AHAM.Services.Commission.API.Application.IntegrationEvents
{
    public class CommissionIntegrationEventService : ICommissionIntegrationEventService
    {
        private readonly CommissionContext _commissionContext;
        private readonly IEventBus _eventBus;
        private readonly IIntegrationEventLogService _eventLogService;
        private readonly ILogger<CommissionIntegrationEventService> _logger;

        public CommissionIntegrationEventService(
            Func<DbConnection, IIntegrationEventLogService> eventLogServiceFactory,
            CommissionContext commissionContext,
            IEventBus eventBus, 
            ILogger<CommissionIntegrationEventService> logger)
        {
            var svcFactory = eventLogServiceFactory ?? throw new ArgumentNullException(nameof(eventLogServiceFactory));
            _commissionContext = commissionContext ?? throw new ArgumentNullException(nameof(commissionContext));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            _eventLogService = svcFactory(_commissionContext.Database.GetDbConnection());
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

            await _eventLogService.SaveEventAsync(evt, _commissionContext.GetCurrentTransaction());
        }
    }
}