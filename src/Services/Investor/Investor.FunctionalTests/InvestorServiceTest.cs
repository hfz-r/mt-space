using System;
using System.Threading.Tasks;
using AHAM.Services.Investor.Grpc;
using Xunit;
using static Investor.FunctionalTests.FakeData;

namespace Investor.FunctionalTests
{
    public class InvestorServiceTest : IClassFixture<TestBase>, IDisposable
    {
        private readonly TestBase _testBase;

        public InvestorServiceTest(TestBase testBase)
        {
            _testBase = testBase;
            _testBase.SetUp();
        }

        [Fact]
        public async Task Get_rebate_grpc_service_should_work_test()
        {
            //Arrange
            var client = new InvestorService.InvestorServiceClient(_testBase.Channel);

            //Act
            var response = await client.GetRebatesAsync(new GetRebatesRequest
            {
                InvestorId = FakeCreateRebateRequest()[0].InvestorId,
                Size = 300
            });
            var responseP = await client.GetRebatesAsync(new GetRebatesRequest {Coa = "CRFR0011", Size = 10});
            var responseA = await client.GetRebatesAsync(new GetRebatesRequest {Size = 1000});

            //Assert
            Assert.True(response.Rebates.Count > 0);
            Assert.True(responseP.Rebates.Count > 0);
            Assert.True(responseA.Rebates.Count > 0);
        }

        [Fact]
        public async Task Create_rebate_grpc_service_should_work_test()
        {
            //Arrange
            var client = new InvestorService.InvestorServiceClient(_testBase.Channel);

            //Act
            using var call = client.CreateRebate();
            foreach (var request in FakeCreateRebateRequest())
            {
                await call.RequestStream.WriteAsync(request);
            }
            await call.RequestStream.CompleteAsync();

            //Assert
            var response = await call;
            Assert.True(response.Status);
        }

        public void Dispose()
        {
            _testBase.TearDown();
        }
    }
}