using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Cortside.HealthMonitor.WebApi.HostedServices {
    /// <summary>
    /// Base timed hosted service
    /// </summary>
    public abstract class TimedHostedService : BackgroundService {
        //Instantiate a Singleton of the Semaphore with a value of 1. This means that only 1 thread can be granted access at a time.
        private static readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

        /// <summary>
        /// 
        /// </summary>
        protected readonly ILogger logger;
        private readonly int interval;
        private readonly bool enabled;

        /// <summary>
        /// Initializes new instance of the Hosted Service
        /// </summary>
        protected TimedHostedService(ILogger logger, TimedServiceConfiguration config) {
            this.logger = logger;
            this.interval = config.Interval;
            this.enabled = config.Enabled;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
            if (enabled) {
                logger.LogInformation($"{this.GetType().Name} is starting with interval of {interval} seconds");

                stoppingToken.Register(() => logger.LogDebug($"{this.GetType().Name} is stopping."));

                while (!stoppingToken.IsCancellationRequested) {
                    await IntervalAsync();
                    await Task.Delay(TimeSpan.FromSeconds(interval), stoppingToken);
                }
                logger.LogInformation($"{this.GetType().Name} is stopping");
            } else {
                logger.LogInformation($"{this.GetType().Name} is disabled");
            }
        }

        private async Task IntervalAsync() {
            //TODO: create correlationId and set in logging context
            logger.LogDebug($"{this.GetType().Name} is working");
            await semaphore.WaitAsync();

            try {
                await ExecuteIntervalAsync().ConfigureAwait(false);
            } catch (Exception ex) {
                logger.LogError(ex, this.GetType().Name);
            } finally {
                semaphore.Release();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected abstract Task ExecuteIntervalAsync();
    }
}
