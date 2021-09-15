using System;
using System.Linq;
using System.Threading.Tasks;
using AHAM.Services.Datamart.API.Infrastructure.Repositories;
using AHAM.Services.Datamart.API.Model.Envelopes;
using AHAM.Services.Datamart.CimbSplit.Grpc;
using AHAM.Services.Datamart.Dtos.Grpc;
using AutoMapper;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace AHAM.Services.Datamart.API.Services
{
    public class CimbService: CimbSplitService.CimbSplitServiceBase
    {
        private readonly ICimbRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<CimbService> _logger;

        public CimbService(ICimbRepository repository, IMapper mapper, ILogger<CimbService> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        public override async Task<GetCimbResponse> GetCimb(GetCimbRequest request, ServerCallContext context)
        {
            try
            {
                _logger.LogInformation("Begin call from method {method} for cimb-split {GetCimbRequest}", context.Method, request);

                var query = new CimbQuery(request.ProductCode, request.AgentId, request.InvestorId, request.FeeType, request.Index, request.Size, request.From);
                var response = await _repository.GetAsync(query);

                context.Status = response.Count > 0
                    ? new Status(StatusCode.OK, $" get cimb-split {request} do exist")
                    : new Status(StatusCode.NotFound, $" get cimb-split {request} do not exist");

                return ToResponse();

                #region Local function

                GetCimbResponse ToResponse() => new GetCimbResponse {Cimbs = {_mapper.ProjectTo<CimbSplitDTO>(response.AsQueryable())}};

                #endregion
            }
            catch (RpcException)
            {
                var error = new RpcException(new Status(StatusCode.Internal, "Invalid Request"));
                throw new Exception(null, error);
            }
        }

        public override async Task<CreateCimbResponse> CreateCimb(IAsyncStreamReader<CreateCimbRequest> requestStream, ServerCallContext context)
        {
            try
            {
                await foreach (var request in requestStream.ReadAllAsync())
                {
                    _logger.LogInformation("Begin call from method {method} for cimb-split {CreateCimbRequest}", context.Method, request);

                    var command = request.Cimb.Select(dto => _mapper.Map<CimbSplitDTO, CimbCommand>(dto));
                    await _repository.UpsertAsync(command);
                }

                return new CreateCimbResponse
                {
                    Status = true,
                    Message = "Ok"
                };
            }
            catch (RpcException)
            {
                var error = new RpcException(new Status(StatusCode.Internal, "Invalid Response"));
                throw new Exception(null, error);
            }
        }
    }
}