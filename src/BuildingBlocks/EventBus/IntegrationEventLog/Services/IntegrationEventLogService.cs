using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AHAM.BuildingBlocks.EventBus.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace AHAM.BuildingBlocks.IntegrationEventLog.Services
{
    public class IntegrationEventLogService : IIntegrationEventLogService, IDisposable
    {
        private readonly IntegrationEventLogContext _context;
        private readonly List<Type> _eventTypes;
        private volatile bool _disposedValue;

        public IntegrationEventLogService(DbConnection connection)
        {
            var conn = connection ?? throw new ArgumentNullException(nameof(connection));

            _context = new IntegrationEventLogContext(new DbContextOptionsBuilder<IntegrationEventLogContext>()
                .UseSqlServer(conn).Options);

            _eventTypes =
#if DEBUG
                Assembly.Load(Assembly.GetCallingAssembly().FullName)
#else
                Assembly.Load(Assembly.GetEntryAssembly().FullName)
#endif
                .GetTypes()
                .Where(x => x.Name.EndsWith(nameof(IntegrationEvent)))
                .ToList();
        }

        #region Private/Protected methods

        private async Task UpdateEventStatusAsync(Guid eventId, EventStateEnum status)
        {
            var eventLogEntry = _context.IntegrationEventLogs.Single(ie => ie.EventId == eventId);
            eventLogEntry.State = status;

            if (status == EventStateEnum.InProgress) eventLogEntry.TimesSent++;

            _context.IntegrationEventLogs.Update(eventLogEntry);

            await _context.SaveChangesAsync();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing) _context?.Dispose();

                _disposedValue = true;
            }
        }

        #endregion

        public async Task<IEnumerable<IntegrationEventLogEntry>> RetrieveEventLogsPendingToPublishAsync(Guid transactionId)
        {
            var tid = transactionId.ToString();

            var result = await _context.IntegrationEventLogs
                .Where(x => x.TransactionId == tid && x.State == EventStateEnum.NotPublished).ToListAsync();

            if (result != null && result.Any())
            {
                return result.OrderBy(o => o.CreationTime).Select(e =>
                    e.DeserializeJsonContent(_eventTypes.Find(t => t.Name == e.EventTypeShortName)));
            }

            return new List<IntegrationEventLogEntry>();
        }

        public async Task SaveEventAsync(IntegrationEvent @event, IDbContextTransaction transaction)
        {
            if (transaction == null) throw new ArgumentNullException(nameof(transaction));

            var eventLogEntry = new IntegrationEventLogEntry(@event, transaction.TransactionId);

            await _context.Database.UseTransactionAsync(transaction.GetDbTransaction());
            await _context.IntegrationEventLogs.AddAsync(eventLogEntry);

            await _context.SaveChangesAsync();
        }

        public async Task MarkEventAsPublishedAsync(Guid eventId)
        {
            await UpdateEventStatusAsync(eventId, EventStateEnum.Published);
        }

        public async Task MarkEventAsInProgressAsync(Guid eventId)
        {
            await UpdateEventStatusAsync(eventId, EventStateEnum.InProgress);
        }

        public async Task MarkEventAsFailedAsync(Guid eventId)
        {
            await UpdateEventStatusAsync(eventId, EventStateEnum.PublishedFailed);
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}