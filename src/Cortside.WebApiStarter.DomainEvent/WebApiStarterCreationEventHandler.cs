using System;
using System.Threading.Tasks;
using Cortside.Common.DomainEvent;
using Cortside.DomainEvent.Events;
using Cortside.WebApiStarter.DomainService;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog.Context;

namespace Cortside.WebApiStarter.DomainEvent {
    /// <summary>
    /// Handles domain event <see cref="WebApiStarterCreationEvent"/>
    /// </summary>
    public class WebApiStarterCreationEventHandler : IDomainEventHandler<WebApiStarterCreationEvent> {

        private readonly IServiceProvider serviceProvider;
        private readonly ILogger<WebApiStarterCreationEventHandler> logger;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <param name="logger"></param>
        public WebApiStarterCreationEventHandler(IServiceProvider serviceProvider, ILogger<WebApiStarterCreationEventHandler> logger) {
            this.serviceProvider = serviceProvider;
            this.logger = logger;
        }


        /// <summary>
        /// Handle message event
        /// </summary>
        /// <param name="event"></param>
        /// <returns></returns>
        public async Task Handle(DomainEventMessage<WebApiStarterCreationEvent> @event) {
            using (LogContext.PushProperty("MessageId", @event.MessageId))
            using (LogContext.PushProperty("CorrelationId", @event.CorrelationId)) {
                await Handle(@event.Data);
            }
        }

        /// <summary>
        /// Handle message
        /// </summary>
        /// <param name="event"></param>
        /// <returns></returns>
        public async Task Handle(WebApiStarterCreationEvent @event) {
            using (LogContext.PushProperty("Parameter", @event.Parameter)) {
                logger.LogDebug($"Handling {typeof(WebApiStarterCreationEvent).Name} for WebApiStarter {@event.Parameter}");

                using (IServiceScope scope = serviceProvider.CreateScope()) {
                    IWebApiStarterService WebApiStarterService = scope.ServiceProvider.GetRequiredService<IWebApiStarterService>();
                    await WebApiStarterService.CreateWebApiStarter(@event.Parameter);
                }

                logger.LogDebug($"Successfully handled {typeof(WebApiStarterCreationEvent).Name} for WebApiStarter {@event.Parameter}");
            }
        }
    }
}
