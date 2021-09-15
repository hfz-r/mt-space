using System;
using AHAM.Services.Datamart.CimbSplit.Grpc;
using AHAM.Services.Datamart.Dtos.Grpc;
using Google.Protobuf.WellKnownTypes;

namespace Datamart.FunctionalTests
{
    internal static class FakeData
    {
        public static CreateCimbRequest[] FakeRequests1()
        {
            var request1 = new CreateCimbRequest
            {
                Cimb =
                {
                    new CimbSplitDTO
                    {
                        Id = "35121",
                        EffectiveFrom = Timestamp.FromDateTime(DateTime.UtcNow),
                        AgentId = "RS20052",
                        InvestorId = "DM0000041",
                        ProductCode = "DAGPOHCT",
                        Plan = "deleted"
                    },
                    new CimbSplitDTO
                    {
                        Id = "35120",
                        EffectiveFrom = Timestamp.FromDateTime(DateTime.UtcNow),
                        AgentId = "RS20015",
                        InvestorId = "DM0000042",
                        ProductCode = "LIMBCWBT"
                    },
                    new CimbSplitDTO
                    {
                        Id = "123",
                        EffectiveFrom = Timestamp.FromDateTime(DateTime.UtcNow),
                        AgentId = "RS20052",
                        InvestorId = "DM0000043",
                        ProductCode = "YEOKYWBT"
                    },
                }
            };

            return new[] {request1};
        }

        public static CreateCimbRequest[] FakeRequests2()
        {
            var request2 = new CreateCimbRequest
            {
                Cimb =
                {
                    new CimbSplitDTO
                    {
                        Id = "",
                        EffectiveFrom = Timestamp.FromDateTime(DateTime.UtcNow),
                        AgentId = "ERR1",
                        InvestorId = "ERR1",
                        ProductCode = ""
                    },
                }
            };

            return new[] {request2};
        }
    }
}