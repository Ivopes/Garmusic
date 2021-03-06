using Garmusic.Interfaces.Utilities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Garmusic.Utilities
{
    public class BackgroundWorker : BackgroundService
    {
        public IBackgroundTaskQueue TaskQueue { get; }
        private readonly ILogger<BackgroundWorker> _logger;
        private readonly IWebHostEnvironment _env;
        public BackgroundWorker(IBackgroundTaskQueue taskQueue, ILogger<BackgroundWorker> logger, IWebHostEnvironment env)
        {
            TaskQueue = taskQueue;
            _logger = logger;
            _env = env;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!_env.IsDevelopment())
            {
                return;
            }

            _logger.LogInformation("Queued Hosted Service is running.");

            await BackgroundProcessing(stoppingToken);
        }
        private async Task BackgroundProcessing(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Yield();

                var workItem = await TaskQueue.DequeueAsync(stoppingToken);

                if (workItem is null)
                {
                    continue;
                }

                try
                {
                    await workItem(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred executing {WorkItem}.", nameof(workItem));
                }
            }
        }
        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Queued Hosted Service is stopping.");

            await base.StopAsync(stoppingToken);
        }
    }
}
