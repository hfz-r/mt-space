using AHAM.Services.Commission.Domain.AggregatesModel.FeeRebateAggregate;
using AHAM.Services.Commission.Infrastructure.Paging;
using MediatR;

namespace AHAM.Services.Commission.API.Application.Queries
{
    public class GetRebatesQuery : IRequest<IPaginate<FeeRebate>>
    {
        public GetRebatesQuery(string investorId, string coa, int index, int size, int from)
        {
            InvestorId = investorId;
            Coa = coa;
            Index = index;
            Size = size;
            From = from;
        }

        public string InvestorId { get; }
        public string Coa { get; }
        public int Index { get; }
        public int Size { get; }
        public int From { get; }
    }
}