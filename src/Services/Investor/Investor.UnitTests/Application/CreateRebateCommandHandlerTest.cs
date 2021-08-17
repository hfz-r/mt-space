using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AHAM.BuildingBlocks.RedisCache;
using AHAM.Services.Investor.API.Application.Commands;
using AHAM.Services.Investor.API.Application.IntegrationEvents;
using AHAM.Services.Investor.Domain.AggregatesModel.FeeRebateAggregate;
using AHAM.Services.Investor.Infrastructure;
using AHAM.Services.Investor.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using static Investor.UnitTests.FakeData;

namespace Investor.UnitTests.Application
{
    public class CreateRebateCommandHandlerTest
    {
        private readonly Mock<IUnitOfWork> _worker;
        private readonly Mock<ICacheManager> _cache;
        private readonly Mock<IInvestorIntegrationEventService> _integrationEventMock;
        private readonly Mock<ILogger<CreateRebateCommandHandler>> _loggerMock;
        private readonly Mock<InvestorContext> _invContextMock;

        public CreateRebateCommandHandlerTest()
        {
            _worker = new Mock<IUnitOfWork>();
            _cache = new Mock<ICacheManager>();
            _integrationEventMock = new Mock<IInvestorIntegrationEventService>();
            _loggerMock = new Mock<ILogger<CreateRebateCommandHandler>>();
            _invContextMock = new Mock<InvestorContext>(new DbContextOptions<InvestorContext>());
        }

        [Fact]
        public async Task Command_handler_should_work_test()
        {
            //Arrange
            var fakeRebates = FakeRebates().ToList();

            _invContextMock.Setup(x => x.Set<FeeRebate>().AddRangeAsync(It.IsAny<IEnumerable<FeeRebate>>(), CancellationToken.None))
                .Callback((IEnumerable<FeeRebate> rebates, CancellationToken c) => fakeRebates.AddRange(rebates))
                .Returns(() => Task.CompletedTask);
            _worker.Setup(x => x.GetRepositoryAsync<FeeRebate>()).Returns(() => new RepositoryAsync<FeeRebate>(_invContextMock.Object, _cache.Object));
            _worker.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(true);

            //Act
            var handler = new CreateRebateCommandHandler(_worker.Object, _integrationEventMock.Object, _loggerMock.Object);
            var result = await handler.Handle(new CreateRebateCommand { Request = FakeCreateRebateRequest() }, CancellationToken.None);

            //Assert
            Assert.True(result);
            Assert.Equal(3, fakeRebates.Count);
        }
    }
}