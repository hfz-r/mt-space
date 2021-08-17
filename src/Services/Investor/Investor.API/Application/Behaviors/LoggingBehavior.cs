using System.Threading;
using System.Threading.Tasks;
using AHAM.BuildingBlocks.EventBus.Extensions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AHAM.Services.Investor.API.Application.Behaviors
{
    public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

        public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            _logger.LogInformation("----- Handling command {CommandName}", request.GetGenericTypeName());
            var response = await next();
            _logger.LogInformation("----- Command {CommandName} handled", request.GetGenericTypeName());

            return response;
        }
    }
}