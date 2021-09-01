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
                        Id = 882,
                        Amc = "AFCX",
                        Agent = "AGENTX",
                        Channel = "CHANNELX",
                        Coa = "CRFR0011A",
                        Currency = "AUD",
                        Drcr = "X",
                        Investor = new InvestorDTO
                        {
                            InvestorId = "0023170-1",
                            InvestorName = "DB (M) NOMINEE (T) S/B",
                            Address = new AddressDTO()
                        },
                        Plan = "X",
                        SetupBy = "test-user",
                        SetupType = "X",
                        SetupDate = Timestamp.FromDateTime(DateTime.UtcNow),
                        Type = "RebateX"
                    },
                    new FeeRebateDTO
                    {
                        Id = 883,
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
                    new FeeRebateDTO
                    {
                        Id = 900,
                        Amc = "AFCY",
                        Agent = "AGENTY",
                        Channel = "CHANNELY",
                        Coa = "COA666Y",
                        Currency = "AUD",
                        Drcr = "Y",
                        Investor = new InvestorDTO
                        {
                            InvestorId = "0023170-1",
                            InvestorName = "DB (M) NOMINEE (T) S/B",
                            Address = new AddressDTO()
                        },
                        Plan = "Y",
                        SetupBy = "test-user",
                        SetupType = "Y",
                        SetupDate = Timestamp.FromDateTime(DateTime.UtcNow),
                        Type = "deleted"
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
                        Id = 884,
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
                    new FeeRebateDTO
                    {
                        Id = 0,
                        Amc = "AFCX",
                        Agent = "AGENTX",
                        Channel = "CHANNELX",
                        Coa = "CRFR0011A",
                        Currency = "AUD",
                        Drcr = "X",
                        Investor = new InvestorDTO
                        {
                            InvestorId = "0000057",
                            InvestorName = "AFFIN HWANG INVESTMENT BANK BERHAD",
                            Address = new AddressDTO()
                        },
                        Plan = "X",
                        SetupBy = "test-user",
                        SetupType = "X",
                        SetupDate = Timestamp.FromDateTime(DateTime.UtcNow),
                        Type = "RebateX"
                    },
                }
            };

            return new[] {req1, req2};
        }
    }
}