using System;
using System.Collections.Generic;
using AHAM.Services.Commission.Domain.AggregatesModel.FeeRebateAggregate;
using AHAM.Services.Commission.Domain.AggregatesModel.InvestorAggregate;
using AHAM.Services.Commission.Dtos.Grpc;
using AHAM.Services.Commission.Investor.Grpc;
using Google.Protobuf.WellKnownTypes;
using Inv = AHAM.Services.Commission.Domain.AggregatesModel.InvestorAggregate.Investor;

namespace Commission.UnitTests
{
    internal static class FakeData
    {
        public static IEnumerable<FeeRebate> FakeRebates()
        {
            return new List<FeeRebate>
            {
                new FeeRebate("AFC", "AGENT1", "CHANNEL1", "CRFR0011", "C", "SEX1", "test-user",
                    "SX", DateTime.UtcNow, "Rebate", "MYR", "0023170-1"),
                new FeeRebate("AFC", "AGENT1", "CHANNEL2", "CRFR0012", "C", "SEX2", "test-user",
                    "SX", DateTime.UtcNow, "Rebate", "USD", "0023170-1"),
            };
        }

        public static IList<FeeRebate> FakeRebatesAndInvestor()
        {
            var rebate1 = new FeeRebate("AFC", "AGENT1", "CHANNEL1", "CRFR0011", "C", "SEX1", "test-user",
                "SX", DateTime.UtcNow, "Rebate", "MYR", "0023170-1");
            rebate1.SetInvestorId(FakeInvestors()[0].InvestorId);

            var rebate2 = new FeeRebate("AFC", "AGENT1", "CHANNEL2", "CRFR0012", "C", "SEX2", "test-user",
                "SX", DateTime.UtcNow, "Rebate", "USD", "0012137");
            rebate2.SetInvestorId(FakeInvestors()[1].InvestorId);

            return new[] {rebate1, rebate2};
        }

        public static IList<Inv> FakeInvestors()
        {
            return new List<Inv>
            {
                new Inv("0023170-1", "DB (M) NOMINEE (T) S/B",
                    new Address("18 Street Fighter Kick Boobs", "Boobs Kicker City", "State 1", "Count 1", "001")),
                new Inv("0012137", "TOKIO MARINE INSURANS (MALAYSIA) BERHAD",
                    new Address("7, Street of Tokio Insurans", "Red Shit Village", "Sungai Taik", "Malaysia", "48050")),
            };
        }

        public static InvestorDTO InvestorDto() => new InvestorDTO
        {
            InvestorId = "0023170-1",
            InvestorName = "DB (M) NOMINEE (T) S/B",
            Address = new AddressDTO()
        };

        public static CreateRebateRequest FakeCreateRebateRequest1()
        {
            var dto = new FeeRebateDTO
            {
                Amc = "A1C",
                Agent = "AGENT1",
                Channel = "CHANNEL1",
                Coa = "CRFR0011",
                Currency = "MYR",
                Drcr = "C",
                InvestorId = InvestorDto().InvestorId,
                Plan = "SEX1",
                SetupBy = "test-user",
                SetupType = "S1",
                SetupDate = Timestamp.FromDateTime(DateTime.UtcNow),
                Type = "Rebate1"
            };

            var request = new CreateRebateRequest();
            request.Rebates.Add(dto);

            return request;
        }

        public static CreateRebateRequest FakeCreateRebateRequest2()
        {
            var dto = new FeeRebateDTO
            {
                Amc = "A2C",
                Agent = "AGENT2",
                Channel = "CHANNEL2",
                Coa = "CRFR0012",
                Currency = "USD",
                Drcr = "D",
                InvestorId = "FakeId",
                Plan = "SEX2",
                SetupBy = "test-user",
                SetupType = "S2",
                SetupDate = Timestamp.FromDateTime(DateTime.UtcNow),
                Type = "Rebate2"
            };

            var request = new CreateRebateRequest();
            request.Rebates.Add(dto);

            return request;
        }
    }
}