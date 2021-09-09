using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AHAM.BuildingBlocks.RedisCache;
using AHAM.BuildingBlocks.RedisCache.CacheKey;
using AHAM.Services.Commission.API.Application.Queries;
using AHAM.Services.Commission.Domain.AggregatesModel.FeeRebateAggregate;
using AHAM.Services.Commission.Domain.SeedWork;
using AHAM.Services.Commission.Infrastructure;
using AHAM.Services.Commission.Infrastructure.Paging;
using AHAM.Services.Commission.Infrastructure.Repositories;
using AHAM.Services.Commission.Investor.Grpc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MockQueryable.Moq;
using Moq;
using Xunit;
using static Commission.UnitTests.FakeData;

namespace Commission.UnitTests.Application
{
    public class GetRebatesQueryHandlerTest
    {
        private readonly Mock<IUnitOfWork> _workerMock;
        private readonly Mock<ICacheManager> _cacheMock;
        private readonly Mock<ILogger<GetRebatesQueryHandler>> _loggerMock;
        private readonly Mock<CommissionContext> _invContextMock;

        public GetRebatesQueryHandlerTest()
        {
            _workerMock = new Mock<IUnitOfWork>();
            _cacheMock = new Mock<ICacheManager>();
            _loggerMock = new Mock<ILogger<GetRebatesQueryHandler>>();
            _invContextMock = new Mock<CommissionContext>(new DbContextOptions<CommissionContext>());
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
                .Setup(x => x.GetAsync(It.IsAny<CacheKey>(), It.IsAny<Func<Task<Paginate<FeeRebate>>>>()))
                .Returns((ICacheManager cache, Func<Task<Paginate<FeeRebate>>> query) => query.Invoke());
        }

        [Fact]
        public async Task Query_handler_cache_should_work_test()
        {
            //Arrange
            CreateMockDbSet(FakeRebates().AsQueryable());

            //Act 
            var fakeQuery = new GetRebatesQuery {Request = new GetRebatesRequest {Size = 10}};

            var handler = new GetRebatesQueryHandler(_workerMock.Object, _loggerMock.Object);
            var fakeResult = await handler.Handle(fakeQuery, CancellationToken.None);

            //Assert
            Assert.Equal(2, fakeResult.Count);
        }

        [Fact]
        public async Task Query_handler_should_resolved_args_test()
        {
            //Arrange
            CreateMockDbSet(FakeRebatesAndInvestor().AsQueryable());

            //Act 
            var fakeQuery = new GetRebatesQuery {Request = new GetRebatesRequest {Size = 10}};
            var fakeQueryParams = new GetRebatesQuery { Request = new GetRebatesRequest { Coa = "CRFR0011" } };

            var handler = new GetRebatesQueryHandler(_workerMock.Object, _loggerMock.Object);
            var fakeResult = await handler.Handle(fakeQuery, CancellationToken.None);
            var fakeResultParams = await handler.Handle(fakeQueryParams, CancellationToken.None);

            //Assert
            Assert.NotEmpty(fakeResult.Items.Select(x => x.InvestorId));
            Assert.Equal(1, fakeResultParams.Count);
        }
    }
}