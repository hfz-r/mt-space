using System;
using System.Collections.Generic;
using AHAM.Services.Dtos.Grpc;
using AHAM.Services.Investor.Domain.AggregatesModel.FeeRebateAggregate;
using AHAM.Services.Investor.Domain.AggregatesModel.InvestorAggregate;
using AHAM.Services.Investor.Grpc;
using Google.Protobuf.WellKnownTypes;
using Inv = AHAM.Services.Investor.Domain.AggregatesModel.InvestorAggregate.Investor;

namespace Investor.UnitTests
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

        public static IList<FeeRebate> FakeIndexesData()
        {
            return new List<FeeRebate>
            {
                new FeeRebate("AFC", "", "", "CR123", "C", "", "", "", DateTime.UtcNow, "Rebate", "EUR", "0000057"),
                new FeeRebate("AFC", "", "", "CR123456", "C", "", "", "", DateTime.UtcNow, "Rebate", "EUR", "0000057"),
                new FeeRebate("AFC", "", "", "CRFR0011C", "C", "", "", "", DateTime.UtcNow, "Rebate", "MYR", "0000057"),
                new FeeRebate("AFC", "", "", "CR456", "C", "", "", "", DateTime.UtcNow, "Rebate", "GBP", "0000057"),
                new FeeRebate("AFC", "", "", "CR789", "C", "", "", "", DateTime.UtcNow, "Rebate", "AUD", "0000057"),
                new FeeRebate("AFC", "", "", "CRPA0005", "C", "", "", "", DateTime.UtcNow, "Rebate", "MYR", "0000057"),
            };
        }

        public static IList<Inv> FakeInvestors()
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
                Investor = InvestorDto(),
                Plan = "SEX1",
                SetupBy = "test-user",
                SetupType = "S1",
                SetupDate = Timestamp.FromDateTime(DateTime.UtcNow),
                Type = "Rebate1"
            };

            var request = new CreateRebateRequest();
            request.InvestorId = InvestorDto().InvestorId;
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
                Investor = InvestorDto(),
                Plan = "SEX2",
                SetupBy = "test-user",
                SetupType = "S2",
                SetupDate = Timestamp.FromDateTime(DateTime.UtcNow),
                Type = "Rebate2"
            };

            var request = new CreateRebateRequest();
            request.InvestorId = "FakeId";
            request.Rebates.Add(dto);

            return request;
        }

        public static CreateRebateRequest FakeIndexesRequest()
        {
            var dto1 = new FeeRebateDTO
            {
                Coa = "CR123",
                Type = "Rebate",
                Amc = "A1",
                Currency = "USD",
                Drcr = "C",
                SetupDate = Timestamp.FromDateTime(DateTime.UtcNow),
            };
            var dto2 = new FeeRebateDTO
            {
                Coa = "CR123456",
                Type = "Rebate",
                Amc = "A2",
                Currency = "EUR",
                Drcr = "C",
                SetupDate = Timestamp.FromDateTime(DateTime.UtcNow),
            };
            var dto3 = new FeeRebateDTO
            {
                Coa = "CR789",
                Type = "Rebate",
                Amc = "A3",
                Currency = "AUD",
                Drcr = "C",
                SetupDate = Timestamp.FromDateTime(DateTime.UtcNow),
            };
            var dto4 = new FeeRebateDTO
            {
                Coa = "CRFR0011C",
                Type = "Rebate",
                Amc = "A4",
                Currency = "MYR",
                Drcr = "C",
                SetupDate = Timestamp.FromDateTime(DateTime.UtcNow),
            };
            var dto5 = new FeeRebateDTO
            {
                Coa = "CRPA0005",
                Type = "Rebate",
                Amc = "A5",
                Currency = "MYR",
                Drcr = "C",
                SetupDate = Timestamp.FromDateTime(DateTime.UtcNow),
            };

            var request = new CreateRebateRequest();
            request.InvestorId = "0000057";
            request.Rebates.Add(dto1);
            request.Rebates.Add(dto2);
            request.Rebates.Add(dto3);
            request.Rebates.Add(dto4);
            request.Rebates.Add(dto5);

            return request;
        }
    }
}