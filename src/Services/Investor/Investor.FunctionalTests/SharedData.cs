using System;
using AHAM.Services.Dtos.Grpc;
using AHAM.Services.Investor.Grpc;
using Google.Protobuf.WellKnownTypes;

namespace Investor.FunctionalTests
{
    internal static class FakeData
    {
        public static CreateRebateRequest[] FakeCreateRebateRequest()
        {
            var req1 = new CreateRebateRequest
            {
                InvestorId = "0023170-1",
                Rebates =
                {
                    new FeeRebateDTO
                    {
                        Amc = "AFC1",
                        Agent = "AGENT1",
                        Channel = "CHANNEL1",
                        Coa = "CRFR0011A",
                        Currency = "AUD",
                        Drcr = "C",
                        Investor = new InvestorDTO
                        {
                            InvestorId = "0023170-1",
                            InvestorName = "DB (M) NOMINEE (T) S/B",
                            Address = new AddressDTO()
                        },
                        Plan = "A",
                        SetupBy = "test-user",
                        SetupType = "A",
                        SetupDate = Timestamp.FromDateTime(DateTime.UtcNow),
                        Type = "RebateA"
                    },
                    new FeeRebateDTO
                    {
                        Amc = "AFC2",
                        Agent = "AGENT2",
                        Channel = "CHANNEL2",
                        Coa = "CRFR0011B",
                        Currency = "USD",
                        Drcr = "D",
                        Investor = new InvestorDTO
                        {
                            InvestorId = "0023170-1",
                            InvestorName = "DB (M) NOMINEE (T) S/B",
                            Address = new AddressDTO()
                        },
                        Plan = "B",
                        SetupBy = "test-user",
                        SetupType = "B",
                        SetupDate = Timestamp.FromDateTime(DateTime.UtcNow),
                        Type = "RebateB"
                    },
                }
            };
            var req2 = new CreateRebateRequest
            {
                InvestorId = "0000057",
                Rebates =
                {
                    new FeeRebateDTO
                    {
                        Amc = "AFC3",
                        Agent = "AGENT3",
                        Channel = "CHANNEL3",
                        Coa = "CRFR0011C",
                        Currency = "MYR",
                        Drcr = "C",
                        Investor = new InvestorDTO
                        {
                            InvestorId = "0000057",
                            InvestorName = "AFFIN HWANG INVESTMENT BANK BERHAD",
                            Address = new AddressDTO()
                        },
                        Plan = "C",
                        SetupBy = "test-user",
                        SetupType = "C",
                        SetupDate = Timestamp.FromDateTime(DateTime.UtcNow),
                        Type = "RebateC"
                    },
                }
            };

            return new[] {req1, req2};
        }
    }
}