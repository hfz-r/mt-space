namespace AHAM.Services.Datamart.API.Model.Envelopes
{
    public class CimbQuery
    {
        public CimbQuery(
            string productCode = "",
            string agentId = "",
            string investorId = "",
            int feeType = 0,
            int index = 0,
            int size = 100,
            int from = 0)
        {
            ProductCode = productCode;
            AgentId = agentId;
            InvestorId = investorId;
            FeeType = feeType;
            Index = index;
            Size = size;
            From = from;
        }

        public string ProductCode { get; }
        public string AgentId { get; }
        public string InvestorId { get; }
        public int FeeType { get; }
        public int Index { get; }
        public int Size { get; }
        public int From { get; }
    }
}