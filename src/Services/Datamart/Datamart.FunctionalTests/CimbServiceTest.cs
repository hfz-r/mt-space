using System;
using System.Linq;
using System.Threading.Tasks;
using AHAM.Services.Datamart.CimbSplit.Grpc;
using Grpc.Core;
using Xunit;
using static Datamart.FunctionalTests.FakeData;

namespace Datamart.FunctionalTests
{
    public class CimbServiceTest : IClassFixture<TestBase>, IDisposable
    {
        private readonly TestBase _testBase;

        public CimbServiceTest(TestBase testBase)
        {
            _testBase = testBase;
            _testBase.SetUp();
        }

        [Fact]
        public async Task Get_cimb_grpc_service_should_work_test()
        {
            //Arrange
            var client = new CimbSplitService.CimbSplitServiceClient(_testBase.Channel);

            //Act
            var response = await client.GetCimbAsync(new GetCimbRequest {Size = 100});
            var responseFt = await client.GetCimbAsync(new GetCimbRequest { FeeType = 10, Size = 1000});

            //Assert
            Assert.Equal(100, response.Cimbs.Count);
            Assert.True(responseFt.Cimbs.All(x => x.FeeType == 10));
        }

        [Fact]
        public async Task Upsert_cimb_grpc_service_should_work_test()
        {
            //Arrange
            var client = new CimbSplitService.CimbSplitServiceClient(_testBase.Channel);

            //Act
            using var call = client.CreateCimb();
            foreach (var request in FakeRequests1())
            {
                await call.RequestStream.WriteAsync(request);
            }
            await call.RequestStream.CompleteAsync();

            //Assert
            var response = await call;
            Assert.True(response.Status);
        }

        [Fact]
        public async Task Cimb_command_grpc_validator_test()
        {
            //Arrange
            var client = new CimbSplitService.CimbSplitServiceClient(_testBase.Channel);

            //Act
            using var call = client.CreateCimb();
            foreach (var request in FakeRequests2())
            {
                await call.RequestStream.WriteAsync(request);
            }

            await call.RequestStream.CompleteAsync();

            //Assert
            await Assert.ThrowsAsync<RpcException>(async () => await call);
        }

        public void Dispose()
        {
            _testBase.TearDown();
        }
    }
}