using System.Threading;
using System.Threading.Tasks;
using AHAM.Services.Investor.API.Application.Commands;
using AHAM.Services.Investor.API.Services;
using AutoMapper;
using Investor.UnitTests.Helpers;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using static Investor.UnitTests.FakeData;

namespace Investor.UnitTests.Services
{
    public class CreateRebateServiceTest
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<ILogger<InvestorService>> _loggerMock;

        public CreateRebateServiceTest()
        {
            _mapperMock = new Mock<IMapper>();
            _mediatorMock = new Mock<IMediator>();
            _loggerMock = new Mock<ILogger<InvestorService>>();
        }

        [Fact]
        public async Task Create_rebate_service_should_work_test()
        {
            //Arrange
            var callContext = TestServerCallContext.Create();

            var fakeRebateRequest = FakeCreateRebateRequest();

            _mediatorMock.Setup(x => x.Send(It.IsAny<IdentifiedCommand<CreateRebateCommand, bool>>(), CancellationToken.None)).ReturnsAsync(true);

            //Act
            var service = new InvestorService(_mapperMock.Object, _mediatorMock.Object, _loggerMock.Object);
            var response = await service.CreateRebate(fakeRebateRequest, callContext);

            //Assert
            Assert.True(response.Status);
        }
    }
}