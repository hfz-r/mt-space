using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AHAM.BuildingBlocks.RedisCache;
using AHAM.BuildingBlocks.RedisCache.CacheKey;
using AHAM.Services.Investor.API.Application.Commands;
using AHAM.Services.Investor.API.Application.IntegrationEvents;
using AHAM.Services.Investor.Domain.AggregatesModel.FeeRebateAggregate;
using AHAM.Services.Investor.Domain.SeedWork;
using AHAM.Services.Investor.Infrastructure;
using AHAM.Services.Investor.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MockQueryable.Moq;
using Moq;
using Xunit;
using static Investor.UnitTests.FakeData;

namespace Investor.UnitTests.Application
{
    public class CreateRebateCommandHandlerTest : IClassFixture<AutoMapperFixture>
    {
        private readonly AutoMapperFixture _fixture;

        private readonly Mock<IUnitOfWork> _workerMock;
        private readonly Mock<ICacheManager> _cacheMock;
        private readonly Mock<IInvestorIntegrationEventService> _integrationEventMock;
        private readonly Mock<ILogger<CreateRebateCommandHandler>> _loggerMock;
        private readonly Mock<InvestorContext> _invContextMock;

        public CreateRebateCommandHandlerTest(AutoMapperFixture fixture)
        {
            _fixture = fixture;

            _workerMock = new Mock<IUnitOfWork>();
            _cacheMock = new Mock<ICacheManager>();
            _integrationEventMock = new Mock<IInvestorIntegrationEventService>();
            _loggerMock = new Mock<ILogger<CreateRebateCommandHandler>>();
            _invContextMock = new Mock<InvestorContext>(new DbContextOptions<InvestorContext>());
        }

        private void CreateMockDbSet<T>(IQueryable<T> fakeData, Action<Mock<InvestorContext>> ctxCb) where T : Entity, IAggregateRoot
        {
            var mockDbSet = fakeData.BuildMockDbSet();
            _invContextMock
                .Setup(x => x.Set<T>())
                .Returns(mockDbSet.Object);
            _workerMock
                .Setup(x => x.GetRepositoryAsync<T>())
                .Returns(() => new RepositoryAsync<T>(_invContextMock.Object, _cacheMock.Object));
            _workerMock.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            _cacheMock
                .Setup(x => x.GetAsync(It.IsAny<CacheKey>(), It.IsAny<Func<Task<IList<T>>>>()))
                .Returns(async (ICacheManager cache, Func<Task<IList<T>>> query) => await query.Invoke());
            ctxCb(_invContextMock);
        }

        [Fact]
        public async Task Command_handler_add_should_work_test()
        {
            //Arrange
            var fakeRequest = FakeCreateRebateRequest2();
            var fakeData = FakeRebatesAndInvestor().ToList();

            CreateMockDbSet(fakeData.AsQueryable(), context =>
                context
                    .Setup(x => x.Set<FeeRebate>().AddRangeAsync(It.IsAny<IEnumerable<FeeRebate>>(), CancellationToken.None))
                    .Callback((IEnumerable<FeeRebate> rebates, CancellationToken c) => fakeData.AddRange(rebates))
                    .Returns(() => Task.CompletedTask));

            //Act
            var handler = new CreateRebateCommandHandler(_workerMock.Object, _fixture.Mapper, _integrationEventMock.Object, _loggerMock.Object);
            var result = await handler.Handle(new CreateRebateCommand(fakeRequest.InvestorId, fakeRequest.Rebates), CancellationToken.None);

            //Assert
            Assert.True(result);
            Assert.Equal(3, fakeData.Count);
        }

        [Fact]
        public async Task Command_handler_update_should_reflect_test()
        {
            //Arrange
            var fakeRequest = FakeCreateRebateRequest1();
            var fakeData = FakeRebatesAndInvestor().ToList();
            var fakeResult = new List<FeeRebate>();

            CreateMockDbSet(fakeData.AsQueryable(), context =>
                context
                    .Setup(x => x.Set<FeeRebate>().UpdateRange(It.IsAny<IEnumerable<FeeRebate>>()))
                    .Callback((IEnumerable<FeeRebate> rebates) => fakeResult.AddRange(rebates)));

            //Act
            var handler = new CreateRebateCommandHandler(_workerMock.Object, _fixture.Mapper, _integrationEventMock.Object, _loggerMock.Object);
            var result = await handler.Handle(new CreateRebateCommand(fakeRequest.InvestorId, fakeRequest.Rebates), CancellationToken.None);

            //Assert
            Assert.True(result);
            Assert.True(fakeResult.Count > 0);
            Assert.Equal("CRFR0011", fakeResult[0].Coa);
        }
    }
}