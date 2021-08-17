#nullable enable
using System;
using Grpc.Net.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Investor.FunctionalTests.Helpers;
using AHAM.Services.Investor.API;

namespace Investor.FunctionalTests
{
    public class TestBase : IDisposable
    {
        private GrpcChannel? _channel;
        private IDisposable? _testContext;

        protected ILoggerFactory LoggerFactory => Fixture.LoggerFactory;

        public InvestorTestFixture<Startup> Fixture { get; }
        public GrpcChannel Channel => _channel ??= CreateChannel();

        public TestBase()
        {
            Fixture = new InvestorTestFixture<Startup>(ConfigureServices);
        }

        #region Private/protected methods

        protected GrpcChannel CreateChannel()
        {
            return GrpcChannel.ForAddress("http://localhost", new GrpcChannelOptions
            {
                LoggerFactory = LoggerFactory,
                HttpHandler = Fixture.HttpMessageHandler
            });
        }

        protected virtual void ConfigureServices(IServiceCollection services)
        {
        }

        #endregion

        public void SetUp()
        {
            _testContext = Fixture.GetTestContext();
        }

        public void TearDown()
        {
            _channel = null;
            _testContext?.Dispose();
        }

        public void Dispose()
        {
            Fixture.Dispose();
        }
    }
}