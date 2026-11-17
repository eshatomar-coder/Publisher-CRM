using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Publisher_CRM.Services
{
    public class BusinessCentralBackgroundWorker : BackgroundService
    {
        private readonly IBusinessCentralSyncService _syncService;
        private readonly ILogger<BusinessCentralBackgroundWorker> _logger;

        public BusinessCentralBackgroundWorker(IBusinessCentralSyncService syncService, ILogger<BusinessCentralBackgroundWorker> logger)
        {
            _syncService = syncService;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Business Central Sync Worker started.");

            // Create a timer that ticks every 5 hours
            using PeriodicTimer timer = new PeriodicTimer(TimeSpan.FromHours(5));

            try
            {
                // Run immediately on startup, then every 5 hours
                do
                {
                    _logger.LogInformation("Starting Business Central Account Sync at {time}", DateTimeOffset.Now);

                    await _syncService.SyncAccountsFromApiAsync();

                    _logger.LogInformation("Completed Business Central Account Sync at {time}", DateTimeOffset.Now);
                }
                while (await timer.WaitForNextTickAsync(stoppingToken));
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Business Central Sync Worker is stopping.");
            }
        }
    }
}
