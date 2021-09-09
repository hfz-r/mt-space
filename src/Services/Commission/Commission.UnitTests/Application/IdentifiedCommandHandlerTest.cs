using System;
using System.Threading;
using System.Threading.Tasks;
using AHAM.Services.Commission.API.Application.Commands;
using AHAM.Services.Commission.Infrastructure.Idempotent;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using static Commission.UnitTests.FakeData;

namespace Commission.UnitTests.Application
{
    public class IdentifiedCommandHandlerTest
    {
        private readonly Mock<IRequestManager> _requestManagerMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<ILogger<IdentifiedCommandHandler<CreateRebateCommand, bool>>> _loggerMock;

        public IdentifiedCommandHandlerTest()
        {
            _requestManagerMock = new Mock<IRequestManager>();
            _mediatorMock = new Mock<IMediator>();
            _loggerMock = new Mock<ILogger<IdentifiedCommandHandler<CreateRebateCommand, bool>>>();
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task Identified_handler_should_work_test(bool exist)
        {
            // Arrange
            var fakeRequest = FakeCreateRebateRequest1();
            var fakeGuid = Guid.NewGuid();
            var fakeCommand = new IdentifiedCommand<CreateRebateCommand, bool>(new CreateRebateCommand {List = fakeRequest.Rebates}, fakeGuid);

            _requestManagerMock.Setup(x => x.ExistAsync(It.IsAny<Guid>())).ReturnsAsync(exist);
            _mediatorMock.Setup(x => x.Send(It.IsAny<IRequest<bool>>(), default)).ReturnsAsync(true);

            //Act
            var handler = new IdentifiedCommandHandler<CreateRebateCommand, bool>(_mediatorMock.Object, _requestManagerMock.Object, _loggerMock.Object);
            var result = await handler.Handle(fakeCommand, CancellationToken.None);

            //Assert
            if (!exist)
            {
                Assert.True(result);
                _mediatorMock.Verify(x => x.Send(It.IsAny<IRequest<bool>>(), default), Times.Once());
            }
            else
            {
                Assert.False(result);
                _mediatorMock.Verify(x => x.Send(It.IsAny<IRequest<bool>>(), default), Times.Never());
            }
        }
    }
}