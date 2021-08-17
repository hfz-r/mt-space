using System;
using AHAM.Services.Investor.Domain.Events;
using AHAM.Services.Investor.Domain.SeedWork;
using Newtonsoft.Json;
using Inv = AHAM.Services.Investor.Domain.AggregatesModel.InvestorAggregate.Investor;

namespace AHAM.Services.Investor.Domain.AggregatesModel.FeeRebateAggregate
{
    public class FeeRebate : Entity, IAggregateRoot
    {
        [JsonProperty] private string _investorId;
        [JsonProperty] private string _currencyId;

        public string COA { get; }
        public string Type { get; }
        public string AMC { get; }
        public string Channel { get; }
        public string Agent { get; }
        public string Plan { get; }
        public string DrCr { get; }
        public string SetupType { get; }
        public DateTime SetupDate { get; }
        public string SetupBy { get; }
        [JsonProperty] public Inv Investor { get; private set; }

        /// <summary>
        /// ef ctor
        /// </summary>
        protected FeeRebate()
        {
        }

        public FeeRebate(string amc, string agent, string channel, string coa, string drcr,
            string investorId, string plan, string setupBy, string setupType, string type,
            string refCurrId = "", string refInvId = "") : this()
        {
            AMC = amc;
            Agent = agent;
            Channel = channel;
            COA = coa;
            DrCr = drcr;
            Plan = plan;
            SetupBy = setupBy;
            SetupDate = DateTime.Now;
            SetupType = setupType;
            Type = type;
            //key ref
            _investorId = refInvId;
            _currencyId = refCurrId;
            //domain event
            AddRebateDomainEvent(investorId);
        }

        #region Private methods

        private void AddRebateDomainEvent(string investorId)
        {
            AddDomainEvent(new RebateCreatedDomainEvent(investorId, this));
        }

        #endregion

        public string GetInvestorId() => _investorId;

        public string GetCurrency() => _currencyId;

        public void SetInvestorId(string code) => _investorId = code;
    }
}