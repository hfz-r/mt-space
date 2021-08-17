using System;
using System.Threading.Tasks;
using AHAM.Services.Investor.Grpc;
using Grpc.Core;
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
            var response = await client.GetRebatesAsync(new GetRebatesRequest {Size = 300});
            var responseP = await client.GetRebatesAsync(new GetRebatesRequest {Coa = "CRFR0011", Size = 10});

            //Assert
            Assert.True(response.Rebates.Count > 0);
            Assert.Single(responseP.Rebates);
        }

        [Fact]
        public async Task Create_rebate_grpc_service_should_work_test()
        {
            //Arrange
            var headers = new Metadata {{"x-requestid", Guid.NewGuid().ToString()}};
            var client = new InvestorService.InvestorServiceClient(_testBase.Channel);

            //Act
            var response = await client.CreateRebateAsync(FakeCreateRebateRequest(), headers);

            //Assert
            Assert.True(response.Status);
        }

        public void Dispose()
        {
            _testBase.TearDown();
        }
    }
}