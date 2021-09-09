using System;
using System.Threading.Tasks;
using AHAM.BuildingBlocks.EventBus.Events;

namespace AHAM.Services.Commission.API.Application.IntegrationEvents
{
    public interface ICommissionIntegrationEventService
    {
        Task PublishEventsThroughEventBusAsync(Guid transactionId);
        Task AddAndSaveEventAsync(IntegrationEvent evt);
    }
}