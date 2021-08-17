using AHAM.Services.Dtos.Grpc;
using AHAM.Services.Investor.Grpc;

namespace Investor.FunctionalTests
{
    internal static class FakeData
    {
        public static CreateRebateRequest FakeCreateRebateRequest() => new CreateRebateRequest
        {
            InvestorId = "0023170-1",
            Rebates =
            {
                new FeeRebateDTO
                {
                    Amc = "AFC",
                    Agent = "AGENT1",
                    Channel = "CHANNEL1",
                    Coa = "CRFR0011",
                    Currency = "MYR",
                    Drcr = "C",
                    Investor = new InvestorDTO
                    {
                        InvestorId = "0023170-1",
                        InvestorName = "DB (M) NOMINEE (T) S/B",
                        Address = new AddressDTO()
                    },
                    Plan = "SX1",
                    SetupBy = "test-user",
                    SetupType = "X",
                    Type = "Rebate"
                }
            }
        };
    }
}
