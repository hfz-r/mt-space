using System.Threading;
using System.Threading.Tasks;
using AHAM.Services.Commission.API.Application.Commands;
using AHAM.Services.Commission.Investor.Grpc;
using AutoMapper;
using Commission.UnitTests.Helpers;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using static Commission.UnitTests.FakeData;
using InvestorService = AHAM.Services.Commission.API.Services.InvestorService;

namespace Commission.UnitTests.Services
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
            var requestStream = new TestAsyncStreamReader<CreateRebateRequest>(callContext);

            _mediatorMock.Setup(x => x.Send(It.IsAny<IdentifiedCommand<CreateRebateCommand, bool>>(), CancellationToken.None)).ReturnsAsync(true);

            //Act
            var service = new InvestorService(_mapperMock.Object, _mediatorMock.Object, _loggerMock.Object);
            using var call = service.CreateRebate(requestStream, callContext);

            requestStream.AddMessage(FakeCreateRebateRequest1());
            requestStream.AddMessage(FakeCreateRebateRequest2());
            requestStream.Complete();

            //Assert
            var response = await call;
            Assert.True(response.Status);
        }
    }
}