using System;
using System.Threading.Tasks;
using AHAM.BuildingBlocks.EventBus.Events;

namespace AHAM.Services.Investor.API.Application.IntegrationEvents
{
    public interface IInvestorIntegrationEventService
    {
        Task PublishEventsThroughEventBusAsync(Guid transactionId);
        Task AddAndSaveEventAsync(IntegrationEvent evt);
    }
}