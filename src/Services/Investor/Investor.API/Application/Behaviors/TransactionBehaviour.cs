using System;
using System.Threading;
using System.Threading.Tasks;
using AHAM.BuildingBlocks.EventBus.Extensions;
using AHAM.Services.Investor.API.Application.IntegrationEvents;
using AHAM.Services.Investor.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Serilog.Context;

namespace AHAM.Services.Investor.API.Application.Behaviors
{
    public class TransactionBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly InvestorContext _investorContext;
        private readonly ILogger<TransactionBehaviour<TRequest, TResponse>> _logger;
        private readonly IInvestorIntegrationEventService _integrationEventService;

        public TransactionBehaviour(InvestorContext investorContext, ILogger<TransactionBehaviour<TRequest, TResponse>> logger, IInvestorIntegrationEventService integrationEventService)
        {
            _investorContext = investorContext ?? throw new ArgumentException(nameof(InvestorContext));
            _logger = logger ?? throw new ArgumentException(nameof(ILogger));
            _integrationEventService = integrationEventService ?? throw new ArgumentException(nameof(IInvestorIntegrationEventService));
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var response = default(TResponse);
            var typeName = request.GetGenericTypeName();

            try
            {
                if (_investorContext.HasActiveTransaction) return await next();

                var strategy = _investorContext.Database.CreateExecutionStrategy();

                await strategy.ExecuteAsync(async () =>
                {
                    Guid transactionId;
                    using (var transaction = await _investorContext.BeginTransactionAsync())
                    using (LogContext.PushProperty("TransactionContext", transaction.TransactionId))
                    {
                        _logger.LogInformation("----- Begin transaction {TransactionId} for {CommandName} ({@Command})", transaction.TransactionId, typeName, request);

                        response = await next();

                        _logger.LogInformation("----- Commit transaction {TransactionId} for {CommandName}", transaction.TransactionId, typeName);

                        await _investorContext.CommitTransactionAsync(transaction);

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