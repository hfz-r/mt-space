using System;
using AHAM.Services.Commission.Domain.AggregatesModel.InvestorAggregate;
using AHAM.Services.Commission.Domain.Events;
using AHAM.Services.Commission.Domain.SeedWork;
using Newtonsoft.Json;

namespace AHAM.Services.Commission.Domain.AggregatesModel.FeeRebateAggregate
{
    public class FeeRebate : Entity, IAggregateRoot
    {
        public string Coa { get; private set; }
        public DateTime SetupDate { get; private set; }
        public string DrCr { get; private set; }
        public string Channel { get; }
        public string Agent { get; }
        public string Type { get; }
        public string Amc { get; }
        public string Plan { get; }
        public string SetupType { get; }
        public string SetupBy { get; }

        [JsonProperty] private string _investorId;
        [JsonProperty] private string _currencyId;
        [JsonProperty] private Investor _investor;

        /// <summary>
        /// ef ctor
        /// </summary>
        protected FeeRebate()
        {
        }

        public FeeRebate(string amc, string agent, string channel, string coa, string drcr,
            string plan, string setupBy, string setupType, DateTime setupDate, 
            string type, string refCurrId = "", string refInvId = "") : this()
        {
            Amc = amc;
            Agent = agent;
            Channel = channel;
            Coa = coa;
            DrCr = drcr;
            Plan = plan;
            SetupBy = setupBy;
            SetupType = setupType;
            SetupDate = setupDate;
            Type = type;
            //key ref
            _investorId = refInvId;
            _currencyId = refCurrId;

            //todo: polish domain event between aggregate
            if (!string.IsNullOrEmpty(refInvId)) AddRebateDomainEvent(refInvId);
        }

        #region Private methods

        private void AddRebateDomainEvent(string investorId)
        {
            AddDomainEvent(new RebateCreatedDomainEvent(investorId, this));
        }

        #endregion

        public string GetCurrency() => _currencyId;
        public (string id, string name) GetInvestor() => (_investor.InvestorId, _investor.InvestorName);

        public void SetCoa(string coa) => Coa = coa;
        public void SetDrCr(string drcr) => DrCr = drcr;
        public void SetSetupDate(DateTime date) => SetupDate = date;
        public void SetCurrency(string currency) => _currencyId = currency;
        public void SetInvestorId(string code) => _investorId = code;
    }
}