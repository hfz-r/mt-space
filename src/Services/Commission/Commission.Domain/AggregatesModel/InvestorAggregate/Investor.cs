using System;
using AHAM.Services.Commission.Domain.SeedWork;
using Newtonsoft.Json;

namespace AHAM.Services.Commission.Domain.AggregatesModel.InvestorAggregate
{
    public class Investor : Entity, IAggregateRoot
    {
        public string InvestorId { get; private set; }
        public string InvestorName { get; private set; }
        [JsonProperty] public Address Address { get; private set; }

        /// <summary>
        /// ef ctor
        /// </summary>
        protected Investor()
        {
        }

        public Investor(string investorId, string investorName, Address address) : this()
        {
            InvestorId = !string.IsNullOrEmpty(investorId) ? investorId : throw new ArgumentNullException(nameof(investorId));
            InvestorName = !string.IsNullOrEmpty(investorName) ? investorName : throw new ArgumentNullException(nameof(investorName));
            Address = address;
        }
    }
}