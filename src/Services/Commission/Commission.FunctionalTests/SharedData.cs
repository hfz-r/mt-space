using System;
using System.Collections.Generic;
using AHAM.Services.Commission.Dtos.Grpc;
using AHAM.Services.Commission.Investor.Grpc;
using Google.Protobuf.WellKnownTypes;

namespace Commission.FunctionalTests
{
    internal static class FakeData
    {
        public static IList<InvestorDTO> InvestorDto() => new List<InvestorDTO>
        {
            new InvestorDTO
            {
                InvestorId = "0023170-1",
                InvestorName = "DB (M) NOMINEE (T) S/B",
                Address = new AddressDTO()
            },
            new InvestorDTO
            {
                InvestorId = "0000057",
                InvestorName = "AFFIN HWANG INVESTMENT BANK BERHAD",
                Address = new AddressDTO()
            }
        };

        public static CreateRebateRequest[] FakeCreateRebateRequest()
        {
            var req1 = new CreateRebateRequest
            {
                Rebates =
                {
                    new FeeRebateDTO
                    {
                        Id = 882,
                        InvestorId = InvestorDto()[0].InvestorId,
                        Amc = "AFCX",
                        Agent = "AGENTX",
                        Channel = "CHANNELX",
                        Coa = "CRFR0011A",
                        Currency = "AUD",
                        Drcr = "X",
                        Plan = "X",
                        SetupBy = "test-user",
                        SetupType = "X",
                        SetupDate = Timestamp.FromDateTime(DateTime.UtcNow),
                        Type = "RebateX"
                    },
                    new FeeRebateDTO
                    {
                        Id = 883,
                        InvestorId = InvestorDto()[0].InvestorId,
                        Amc = "AFC2",
                        Agent = "AGENT2",
                        Channel = "CHANNEL2",
                        Coa = "CRFR0011B",
                        Currency = "USD",
                        Drcr = "D",
                        Plan = "B",
                        SetupBy = "test-user",
                        SetupType = "B",
                        SetupDate = Timestamp.FromDateTime(DateTime.UtcNow),
                        Type = "RebateB"
                    },
                    new FeeRebateDTO
                    {
                        Id = 900,
                        InvestorId = InvestorDto()[0].InvestorId,
                        Amc = "AFCY",
                        Agent = "AGENTY",
                        Channel = "CHANNELY",
                        Coa = "COA666Y",
                        Currency = "AUD",
                        Drcr = "Y",
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
                Rebates =
                {
                    new FeeRebateDTO
                    {
                        Id = 884,
                        InvestorId = InvestorDto()[1].InvestorId,
                        Amc = "AFC3",
                        Agent = "AGENT3",
                        Channel = "CHANNEL3",
                        Coa = "CRFR0011C",
                        Currency = "MYR",
                        Drcr = "C",
                        Plan = "C",
                        SetupBy = "test-user",
                        SetupType = "C",
                        SetupDate = Timestamp.FromDateTime(DateTime.UtcNow),
                        Type = "RebateC"
                    },
                    new FeeRebateDTO
                    {
                        Id = 0,
                        InvestorId = InvestorDto()[1].InvestorId,
                        Amc = "AFCX",
                        Agent = "AGENTX",
                        Channel = "CHANNELX",
                        Coa = "CRFR0011A",
                        Currency = "AUD",
                        Drcr = "X",
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