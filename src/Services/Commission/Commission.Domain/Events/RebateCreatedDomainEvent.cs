using AHAM.Services.Commission.Domain.AggregatesModel.FeeRebateAggregate;
using MediatR;

namespace AHAM.Services.Commission.Domain.Events
{
    /// <summary>
    /// Event used when rebate is created
    /// </summary>
    public class RebateCreatedDomainEvent : INotification
    {
        public string InvestorId { get; }
        public FeeRebate Rebate { get; }

        public RebateCreatedDomainEvent(string investorId, FeeRebate rebate)
        {
            InvestorId = investorId;
            Rebate = rebate;
        }
    }
}