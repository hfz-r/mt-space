using System.Collections.Generic;
using AHAM.Services.Dtos.Grpc;
using MediatR;

namespace AHAM.Services.Investor.API.Application.Commands
{
    public class CreateRebateCommand : IRequest<bool>
    {
        public string InvestorId { get; }
        public IList<FeeRebateDTO> List { get; }

        public CreateRebateCommand(string investorId, IList<FeeRebateDTO> list)
        {
            InvestorId = investorId;
            List = list;
        }
    }
}