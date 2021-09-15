using System.Linq;
using System.Threading.Tasks;
using AHAM.BuildingBlocks.EventBus.Extensions;
using AHAM.Services.Datamart.API.Infrastructure.Exceptions;
using FluentValidation;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.Extensions.Logging;

namespace AHAM.Services.Datamart.API.Infrastructure.Grpc
{
    // todo: factorize helper method
    public class ValidatorInterceptor : Interceptor
    {
        private readonly ILogger<ValidatorInterceptor> _logger;
        private readonly IValidator[] _validators;

        public ValidatorInterceptor(ILogger<ValidatorInterceptor> logger, IValidator[] validators)
        {
            _logger = logger;
            _validators = validators;
        }

        public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
            TRequest request, 
            ServerCallContext context,
            UnaryServerMethod<TRequest, TResponse> continuation)
        {
            var typeName = request.GetType().GetGenericTypeName();

            _logger.LogInformation("----- Validating request {CommandType}", typeName);

            var failures = _validators
                .Select(v => v.Validate(request))
                .SelectMany(result => result.Errors)
                .Where(error => error != null)
                .ToList();

            if (!failures.Any()) return await continuation(request, context);

            _logger.LogWarning("Validation errors - {CommandType} - Request: {@Command} - Errors: {@ValidationErrors}", typeName, request, failures);

            throw new DomainException($"Command Validation Errors for type {typeof(TRequest).Name}", new ValidationException("Validation exception", failures));
        }

        public override async Task<TResponse> ClientStreamingServerHandler<TRequest, TResponse>(
            IAsyncStreamReader<TRequest> requestStream,
            ServerCallContext context,
            ClientStreamingServerMethod<TRequest, TResponse> continuation)
        {
            var validatingRequestStream = new ValidatingAsyncStreamReader<TRequest>(requestStream, request =>
            {
                var typeName = request.GetType().GetGenericTypeName();

                _logger.LogInformation("----- Validating request {CommandType}", typeName);

                var failures = _validators
                    .Select(v => v.Validate(request))
                    .SelectMany(result => result.Errors)
                    .Where(error => error != null)
                    .ToList();

                if (!failures.Any()) return Task.CompletedTask;
                
                _logger.LogWarning("Validation errors - {CommandType} - Request: {@Command} - Errors: {@ValidationErrors}", typeName, request, failures);
                
                throw new DomainException($"Command Validation Errors for type {typeof(TRequest).Name}", new ValidationException("Validation exception", failures));
            });

            return await continuation(validatingRequestStream, context);
        }
    }
}