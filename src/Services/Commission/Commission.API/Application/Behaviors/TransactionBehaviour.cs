using System;
using System.Threading;
using System.Threading.Tasks;
using AHAM.BuildingBlocks.EventBus.Extensions;
using AHAM.Services.Commission.API.Application.IntegrationEvents;
using AHAM.Services.Commission.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Serilog.Context;

namespace AHAM.Services.Commission.API.Application.Behaviors
{
    public class TransactionBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly CommissionContext _commissionContext;
        private readonly ILogger<TransactionBehaviour<TRequest, TResponse>> _logger;
        private readonly ICommissionIntegrationEventService _integrationEventService;

        public TransactionBehaviour(CommissionContext commissionContext, ILogger<TransactionBehaviour<TRequest, TResponse>> logger, ICommissionIntegrationEventService integrationEventService)
        {
            _commissionContext = commissionContext ?? throw new ArgumentException(nameof(CommissionContext));
            _logger = logger ?? throw new ArgumentException(nameof(ILogger));
            _integrationEventService = integrationEventService ?? throw new ArgumentException(nameof(ICommissionIntegrationEventService));
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var response = default(TResponse);
            var typeName = request.GetGenericTypeName();

            try
            {
                if (_commissionContext.HasActiveTransaction) return await next();

                var strategy = _commissionContext.Database.CreateExecutionStrategy();

                await strategy.ExecuteAsync(async () =>
                {
                    Guid transactionId;
                    using (var transaction = await _commissionContext.BeginTransactionAsync())
                    using (LogContext.PushProperty("TransactionContext", transaction.TransactionId))
                    {
                        _logger.LogInformation("----- Begin transaction {TransactionId} for {CommandName} ({@Command})", transaction.TransactionId, typeName, request);

                        response = await next();

                        _logger.LogInformation("----- Commit transaction {TransactionId} for {CommandName}", transaction.TransactionId, typeName);

                        await _commissionContext.CommitTransactionAsync(transaction);

                        transactionId = transaction.TransactionId;
                    }

                    await _integrationEventService.PublishEventsThroughEventBusAsync(transactionId);
                });

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ERROR Handling transaction for {CommandName} ({@Command})", typeName, request);

                throw;
            }
        }
    }
}