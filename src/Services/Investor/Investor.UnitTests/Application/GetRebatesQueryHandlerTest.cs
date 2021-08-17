using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AHAM.BuildingBlocks.RedisCache;
using AHAM.BuildingBlocks.RedisCache.CacheKey;
using AHAM.Services.Investor.API.Application.Queries;
using AHAM.Services.Investor.Domain.SeedWork;
using AHAM.Services.Investor.Grpc;
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
    public class GetRebatesQueryHandlerTest
    {
        private readonly Mock<IUnitOfWork> _workerMock;
        private readonly Mock<ICacheManager> _cacheMock;
        private readonly Mock<ILogger<GetRebatesQueryHandler>> _loggerMock;
        private readonly Mock<InvestorContext> _invContextMock;

        public GetRebatesQueryHandlerTest()
        {
            _workerMock = new Mock<IUnitOfWork>();
            _cacheMock = new Mock<ICacheManager>();
            _loggerMock = new Mock<ILogger<GetRebatesQueryHandler>>();
            _invContextMock = new Mock<InvestorContext>(new DbContextOptions<InvestorContext>());
        }

        private void CreateMockDbSet<T>(IQueryable<T> fakeData) where T : Entity, IAggregateRoot
        {
            var mockDbSet = fakeData.BuildMockDbSet();
            _invContextMock
                .Setup(x => x.Set<T>())
                .Returns(mockDbSet.Object);
            _workerMock
                .Setup(x => x.GetRepositoryAsync<T>())
                .Returns(() => new RepositoryAsync<T>(_invContextMock.Object, _cacheMock.Object));
            _cacheMock
                .Setup(x => x.GetAsync(It.IsAny<CacheKey>(), It.IsAny<Func<Task<IList<T>>>>()))
                .Returns(async (ICacheManager cache, Func<Task<IList<T>>> query) => await query.Invoke());
        }

        [Fact]
        public async Task Query_handler_should_work_test()
        {
            //Arrange
            CreateMockDbSet(FakeRebates().AsQueryable());

            //Act 
            var fakeQuery = new GetRebatesQuery {Request = new GetRebatesRequest {Size = 10}};
            var fakeQueryParams = new GetRebatesQuery {Request = new GetRebatesRequest {Coa = "CRFR0011"}};

            var handler = new GetRebatesQueryHandler(_workerMock.Object, _loggerMock.Object);

            var fakeResult = await handler.Handle(fakeQuery, CancellationToken.None);
            var fakeResultParams = await handler.Handle(fakeQueryParams, CancellationToken.None);

            //Assert
            Assert.Equal(2, fakeResult.Count);
            Assert.True(fakeResultParams.Count >= 1);
        }

        [Fact]
        public async Task Query_handler_add_aggregate_should_work_test()
        {
            //Arrange
            CreateMockDbSet(FakeRebatesAndInvestor().AsQueryable());

            //Act 
            var fakeQuery = new GetRebatesQuery {Request = new GetRebatesRequest {Size = 10}};

            var handler = new GetRebatesQueryHandler(_workerMock.Object, _loggerMock.Object);
            var fakeResult = await handler.Handle(fakeQuery, CancellationToken.None);

            //Assert
            Assert.NotEmpty(fakeResult.Items.Select(x => x.GetInvestorId()));
        }
    }
}