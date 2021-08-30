using System;
using AHAM.Services.Investor.Infrastructure.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AHAM.BuildingBlocks.RedisCache;
using AHAM.BuildingBlocks.RedisCache.CacheKey;
using AHAM.Services.Investor.API.Application.Queries;
using AHAM.Services.Investor.Domain.SeedWork;
using AHAM.Services.Investor.Grpc;
using AHAM.Services.Investor.Infrastructure;
using AHAM.Services.Investor.Infrastructure.Paging;
using Grpc.Core;
using Investor.UnitTests.Helpers;
using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using Xunit;
using static Investor.UnitTests.FakeData;
using Inv = AHAM.Services.Investor.Domain.AggregatesModel.InvestorAggregate.Investor;
using InvestorService = AHAM.Services.Investor.API.Services.InvestorService;

namespace Investor.UnitTests.Services
{
    public class GetRebatesServiceTest : IClassFixture<AutoMapperFixture>
    {
        private readonly AutoMapperFixture _fixture;

        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IUnitOfWork> _workerMock;
        private readonly Mock<ICacheManager> _cacheMock;
        private readonly Mock<ILogger<InvestorService>> _loggerMock;
        private readonly Mock<InvestorContext> _invContextMock;

        public GetRebatesServiceTest(AutoMapperFixture fixture)
        {
            _fixture = fixture;

            _mediatorMock = new Mock<IMediator>();
            _workerMock = new Mock<IUnitOfWork>();
            _cacheMock = new Mock<ICacheManager>();
            _loggerMock = new Mock<ILogger<InvestorService>>();
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
                .Setup(x => x.GetAsync(It.IsAny<CacheKey>(), It.IsAny<Func<Task<Inv>>>()))
                .Returns(async (ICacheManager cache, Func<Task<Inv>> query) => await query.Invoke());
        }

        [Fact]
        public async Task Get_rebate_service_should_work_test()
        {
            //Arrange
            var callContext = TestServerCallContext.Create();
            var fakeData = FakeInvestors().AsQueryable();
            CreateMockDbSet(fakeData);

            //rebate request mock
            var fakeRebateRequest = new GetRebatesRequest {Coa = "CRFR0011"};
            var fakeRebateResult = FakeRebatesAndInvestor()
                .Where(x => x.Coa == fakeRebateRequest.Coa)
                .ToPaginate(0, 20);

            _mediatorMock
                .Setup(x => x.Send(It.IsAny<GetRebatesQuery>(), CancellationToken.None))
                .ReturnsAsync(() => fakeRebateResult);

            //Act
            var service = new InvestorService(_fixture.Mapper, _mediatorMock.Object, _loggerMock.Object);
            var response = await service.GetRebates(fakeRebateRequest, callContext);

            //Assert
            Assert.Equal(StatusCode.OK, callContext.Status.StatusCode);
            Assert.True(response.Rebates.Count >= 1);
        }
    }
}