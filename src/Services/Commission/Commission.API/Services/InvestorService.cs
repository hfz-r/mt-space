using System;
using System.Linq;
using System.Threading.Tasks;
using AHAM.Services.Commission.API.Application.Commands;
using AHAM.Services.Commission.API.Application.Queries;
using AHAM.Services.Commission.API.Extensions;
using AHAM.Services.Commission.Dtos.Grpc;
using AHAM.Services.Commission.Investor.Grpc;
using AutoMapper;
using Grpc.Core;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AHAM.Services.Commission.API.Services
{
    public class InvestorService : Investor.Grpc.InvestorService.InvestorServiceBase
    {
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly ILogger<InvestorService> _logger;

        public InvestorService(IMapper mapper, IMediator mediator, ILogger<InvestorService> logger)
        {
            _mapper = mapper;
            _mediator = mediator;
            _logger = logger;
        }

        public override async Task<GetRebatesResponse> GetRebates(GetRebatesRequest request, ServerCallContext context)
        {
            try
            {
                _logger.LogInformation("Begin call from method {method} for investor get rebates {GetRebatesRequest}", context.Method, request);

                var response = await _mediator.Send(new GetRebatesQuery {Request = request});

                context.Status = response.Count > 0
                    ? new Status(StatusCode.OK, $" get rebates {request} do exist")
                    : new Status(StatusCode.NotFound, $" get rebates {request} do not exist");

                return ToResponse();

                #region Local function

                GetRebatesResponse ToResponse()
                {
                    return new GetRebatesResponse
                    {
                        Rebates = {_mapper.ProjectTo<FeeRebateDTO>(response.Items.AsQueryable())},
                    };
                }

                #endregion
            }
            catch (RpcException)
            {
                var error = new RpcException(new Status(StatusCode.Internal, "Invalid Request"));
                throw new Exception(null, error);
            }
        }

        public override async Task<GetInvestorsResponse> GetInvestors(GetInvestorsRequest request, ServerCallContext context)
        {
            try
            {
                _logger.LogInformation("Begin call from method {method} for investor {GetInvestorsRequest}", context.Method, request);

                var response = await _mediator.Send(new GetInvestorsQuery {Request = request});

                context.Status = response.Count > 0
                    ? new Status(StatusCode.OK, $" get investor {request} do exist")
                    : new Status(StatusCode.NotFound, $" get investor {request} do not exist");

                return ToResponse();

                #region Local function

                GetInvestorsResponse ToResponse()
                {
                    return new GetInvestorsResponse
                    {
                        Investors = {_mapper.ProjectTo<InvestorDTO>(response.Items.AsQueryable())}
                    };
                }

                #endregion
            }
            catch (RpcException)
            {
                var error = new RpcException(new Status(StatusCode.Internal, "Invalid Request"));
                throw new Exception(null, error);
            }
        }

        public override async Task<CreateRebateResponse> CreateRebate(IAsyncStreamReader<CreateRebateRequest> requestStream, ServerCallContext context)
        {
            try
            {
                await foreach (var request in requestStream.ReadAllAsync())
                {
                    _logger.LogInformation("Begin call from method {method} for investor create rebate {CreateRebateRequest}", context.Method, request);

                    var command = new IdentifiedCommand<CreateRebateCommand, bool>(new CreateRebateCommand {List = request.Rebates}, context.GetRequestIdHeader());
                    await _mediator.Send(command);
                }

                return new CreateRebateResponse
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