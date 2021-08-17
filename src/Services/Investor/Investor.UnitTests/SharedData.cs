using System.Collections.Generic;
using AHAM.Services.Dtos.Grpc;
using AHAM.Services.Investor.Domain.AggregatesModel.FeeRebateAggregate;
using AHAM.Services.Investor.Domain.AggregatesModel.InvestorAggregate;
using AHAM.Services.Investor.Grpc;
using Inv = AHAM.Services.Investor.Domain.AggregatesModel.InvestorAggregate.Investor;

namespace Investor.UnitTests
{
    internal static class FakeData
    {
        public static IEnumerable<FeeRebate> FakeRebates()
        {
            return new List<FeeRebate>
            {
                new FeeRebate("AFC", "AGENT1", "CHANNEL1", "CRFR0011", "C", "0023170-1", "SEX1", "test-user",
                    "SX", "Rebate", "MYR"),
                new FeeRebate("AFC", "AGENT1", "CHANNEL2", "CRFR0012", "C", "0023170-1", "SEX2", "test-user",
                    "SX", "Rebate", "USD"),
            };
        }

        public static IEnumerable<FeeRebate> FakeRebatesAndInvestor()
        {
            var rebate1 = new FeeRebate("AFC", "AGENT1", "CHANNEL1", "CRFR0011", "C", "0023170-1", "SEX1", "test-user",
                "SX", "Rebate", "MYR");
            rebate1.SetInvestorId("0023170-1");

            var rebate2 = new FeeRebate("AFC", "AGENT1", "CHANNEL2", "CRFR0012", "C", "0023170-1", "SEX2", "test-user",
                "SX", "Rebate", "USD");
            rebate2.SetInvestorId("0012137");

            return new[] {rebate1, rebate2};
        }

        public static IEnumerable<Inv> FakeInvestors()
        {
            return new List<Inv>
            {
                new Inv("0023170-1", "DB (M) NOMINEE (T) S/B", new Address("18 Street Fighter Kick Boobs", "Boobs Kicker City", "State 1", "Count 1", "001")),
                new Inv("0012137", "TOKIO MARINE INSURANS (MALAYSIA) BERHAD", new Address("7, Street of Tokio Insurans", "Red Shit Village", "Sungai Taik", "Malaysia", "48050")),
            };
        }

        public static InvestorDTO InvestorDto() => new InvestorDTO
        {
            InvestorId = "0023170-1",
            InvestorName = "DB (M) NOMINEE (T) S/B",
            Address = new AddressDTO()
        };

        public static CreateRebateRequest FakeCreateRebateRequest() => new CreateRebateRequest
        {
            InvestorId = InvestorDto().InvestorId,
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
                    Investor = InvestorDto(),
                    Plan = "SEX1",
                    SetupBy = "test-user",
                    SetupType = "SX",
                    Type = "Rebate"
                }
            }
        };
    }
}