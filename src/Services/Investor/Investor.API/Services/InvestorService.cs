using System.Linq;
using System.Threading.Tasks;
using AHAM.Services.Dtos.Grpc;
using AHAM.Services.Investor.API.Application.Commands;
using AHAM.Services.Investor.API.Application.Queries;
using AHAM.Services.Investor.API.Extensions;
using AHAM.Services.Investor.Grpc;
using AutoMapper;
using Grpc.Core;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AHAM.Services.Investor.API.Services
{
    public class InvestorService : Grpc.InvestorService.InvestorServiceBase
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
            _logger.LogInformation("Begin call from method {method} for investor get rebates {GetRebatesRequest}", context.Method, request);

            var response = await _mediator.Send(new GetRebatesQuery {Request = request});

            context.Status = response.Count > 0
                ? new Status(StatusCode.OK, $" ordering get orders {request} do exist")
                : new Status(StatusCode.NotFound, $" ordering get orders {request} do not exist");

            return ToResponse();

            #region Local function

            GetRebatesResponse ToResponse()
            {
                return new GetRebatesResponse
                {
                    Rebates = {_mapper.ProjectTo<FeeRebateDTO>(response.Items.AsQueryable())},
                    PageCount = response.Pages
                };
            }

            #endregion
        }

        public override async Task<CreateRebateResponse> CreateRebate(CreateRebateRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Begin call from method {method} for investor create rebate {CreateRebateRequest}", context.Method, request);

            var command = new IdentifiedCommand<CreateRebateCommand, bool>(new CreateRebateCommand {Request = request}, context.GetRequestIdHeader());
            var response = await _mediator.Send(command);

            return response
                ? new CreateRebateResponse
                {
                    Status = true,
                    Message = "Ok"
                }
                : throw new RpcException(new Status(StatusCode.Internal, "Invalid Response"));
        }
    }
}