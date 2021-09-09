using System.Threading;
using System.Threading.Tasks;
using AHAM.Services.Commission.API.Application.Queries;
using AHAM.Services.Commission.Infrastructure.Paging;
using AHAM.Services.Commission.Investor.Grpc;
using Commission.UnitTests.Helpers;
using Grpc.Core;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using static Commission.UnitTests.FakeData;
using InvestorService = AHAM.Services.Commission.API.Services.InvestorService;

namespace Commission.UnitTests.Services
{
    public class GetInvestorsServiceTest : IClassFixture<AutoMapperFixture>
    {
        private readonly AutoMapperFixture _fixture;

        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<ILogger<InvestorService>> _loggerMock;

        public GetInvestorsServiceTest(AutoMapperFixture fixture)
        {
            _fixture = fixture;

            _mediatorMock = new Mock<IMediator>();
            _loggerMock = new Mock<ILogger<InvestorService>>();
        }

        [Fact]
        public async Task Get_rebate_service_should_work_test()
        {
            //Arrange
            var callContext = TestServerCallContext.Create();
            var fakeRequest = new GetInvestorsRequest();

            var fakeResult = FakeInvestors().ToPaginate(0, 20);

            _mediatorMock
                .Setup(x => x.Send(It.IsAny<GetInvestorsQuery>(), CancellationToken.None))
                .ReturnsAsync(() => fakeResult);

            //Act
            var service = new InvestorService(_fixture.Mapper, _mediatorMock.Object, _loggerMock.Object);
            var response = await service.GetInvestors(fakeRequest, callContext);

            //Assert
            Assert.Equal(StatusCode.OK, callContext.Status.StatusCode);
            Assert.Equal(2, response.Investors.Count);
        }
    }
}