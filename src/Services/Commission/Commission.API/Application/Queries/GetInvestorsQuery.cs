using AHAM.Services.Commission.Infrastructure.Paging;
using MediatR;
using Inv = AHAM.Services.Commission.Domain.AggregatesModel.InvestorAggregate.Investor;

namespace AHAM.Services.Commission.API.Application.Queries
{
    public class GetInvestorsQuery : IRequest<IPaginate<Inv>>
    {
        public GetInvestorsQuery(string investorId, string investorName, int index, int size, int from)
        {
            InvestorId = investorId;
            InvestorName = investorName;
            Index = index;
            Size = size;
            From = from;
        }

        public string InvestorId { get; }
        public string InvestorName { get; }
        public int Index { get; }
        public int Size { get; }
        public int From { get; }
    }
}