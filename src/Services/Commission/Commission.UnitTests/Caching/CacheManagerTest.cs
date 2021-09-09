using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AHAM.BuildingBlocks.RedisCache;
using AHAM.BuildingBlocks.RedisCache.CacheKey;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Moq;
using Newtonsoft.Json;
using Xunit;
using static Commission.UnitTests.FakeData;

namespace Commission.UnitTests.Caching
{
    public class CacheManagerTest
    {
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly Mock<IDistributedCache> _distributedCacheMock;

        public CacheManagerTest()
        {
            _configurationMock = new Mock<IConfiguration>();
            _distributedCacheMock = new Mock<IDistributedCache>();
        }

        private void SetupCacheMock<T>(CacheKey fakeCacheKey, IList<T> fakeData, bool clear = false) where T : class
        {
            var fakeCacheIn = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(fakeData));

            if (clear)
            {
                _distributedCacheMock
                    .Setup(x => x.RemoveAsync(It.IsAny<string>(), CancellationToken.None))
                    .Returns(() => Task.CompletedTask);
            }
            else
            {
                _distributedCacheMock
                    .Setup(x => x.GetAsync(fakeCacheKey.Key, CancellationToken.None))
                    .ReturnsAsync(fakeCacheIn);
            }

            _distributedCacheMock
                .Setup(x => x.SetAsync(fakeCacheKey.Key, fakeCacheIn,
                    It.IsAny<DistributedCacheEntryOptions>(), CancellationToken.None))
                .Returns(() => Task.CompletedTask);
        }

        [Fact]
        public async Task Can_set_and_get_object_from_cache()
        {
            //Arrange
            var fakeCacheKey = new CacheKey("some_key_1");
            var fakeResult = FakeInvestors().ToList();
            SetupCacheMock(fakeCacheKey, fakeResult);

            //Act
            var cacheManager = new CacheManager(_configurationMock.Object, _distributedCacheMock.Object);

            await cacheManager.SetAsync(fakeCacheKey, fakeResult);
            var result = await cacheManager.GetAsync(fakeCacheKey, () => fakeResult);

            //Assert
            Assert.Equal(fakeResult.GetType(), result.GetType());
            Assert.Equal(fakeResult.Count, result.Count);
        }

        [Fact]
        public async Task Can_validate_whether_object_is_cached()
        {
            //Arrange
            var fakeCacheKey = new CacheKey("some_key_2");
            var fakeResult1 = FakeInvestors().ToList();
            var fakeResult2 = FakeInvestors().Where(x => x.InvestorId == "0012137").ToList();
            SetupCacheMock(fakeCacheKey, fakeResult1);

            //Act
            var cacheManager = new CacheManager(_configurationMock.Object, _distributedCacheMock.Object);

            var result1 = await cacheManager.GetAsync(fakeCacheKey, () => fakeResult1);
            var result2 = await cacheManager.GetAsync(fakeCacheKey, () => fakeResult2);

            //Assert
            Assert.Equal(fakeResult1.Count, result1.Count);
            Assert.Equal(fakeResult1.Count, result2.Count);
            Assert.NotEqual( fakeResult2.Count, result2.Count);
        }

        [Fact]
        public async Task Can_clear_cache()
        {
            //Arrange
            var fakeCacheKey = new CacheKey("some_key_1");
            var fakeResult = FakeInvestors().ToList();
            SetupCacheMock(fakeCacheKey, fakeResult, true);

            //Act
            var cacheManager = new CacheManager(_configurationMock.Object, _distributedCacheMock.Object);

            await cacheManager.SetAsync(fakeCacheKey, fakeResult);
            await cacheManager.ClearAsync();

            var result = await cacheManager.GetAsync(fakeCacheKey, () => Task.FromResult((object)null));

            //Assert
            Assert.Null(result);
        }
    }
}