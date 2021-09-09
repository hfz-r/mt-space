using System;
using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Commission.FunctionalTests.Helpers
{
    public class CommissionTestContext<TStartup> : IDisposable where TStartup : class
    {
        private readonly Stopwatch _stopwatch;
        private readonly CommissionTestFixture<TStartup> _fixture;

        public CommissionTestContext(CommissionTestFixture<TStartup> fixture)
        {
            _stopwatch = Stopwatch.StartNew();

            _fixture = fixture;
            _fixture.LoggedMessage += WriteMessage;
        }

        private void WriteMessage(LogLevel logLevel, string category, EventId eventId, string message, Exception exception)
        {
            Console.WriteLine($"{_stopwatch.Elapsed.TotalSeconds:N3}s {category} - {logLevel}: {message}");
        }

        public void Dispose()
        {
            _fixture.LoggedMessage -= WriteMessage;
        }
    }
}