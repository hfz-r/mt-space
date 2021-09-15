namespace AHAM.Services.Datamart.API.Model.Envelopes
{
    public class CimbCommand
    {
        public CimbCommand(string id, string effectiveFrom, string productCode, string agentId, string investorId, string plan)
        {
            Id = id;
            EffectiveFrom = effectiveFrom;
            ProductCode = productCode;
            AgentId = agentId;
            InvestorId = investorId;
            Plan = plan;
        }

        public string Id { get; }
        public string EffectiveFrom { get; }
        public string ProductCode { get; }
        public string AgentId { get; }
        public string InvestorId { get; }
        public string Plan { get; }
    }
}