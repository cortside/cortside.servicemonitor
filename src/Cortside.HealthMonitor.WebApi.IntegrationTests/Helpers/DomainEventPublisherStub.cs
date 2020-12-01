using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Cortside.Common.DomainEvent;

namespace Cortside.HealthMonitor.WebApi.IntegrationTests.Helpers {
    public class DomainEventPublisherStub : IDomainEventPublisher {
        public DomainEventError Error { get; set; }

        public event PublisherClosedCallback Closed;

        private readonly List<object> queue = new List<object>();

        public ReadOnlyCollection<object> Queue {
            get {
                return queue.AsReadOnly();
            }
        }

        public Task SendAsync<T>(T @event) where T : class {
            queue.Add(@event);
            Closed?.Invoke(this, null);
            return Task.CompletedTask;
        }

        public void ClearQueue() {
            queue.Clear();
        }

        public Task SendAsync<T>(string eventType, string address, T @event) where T : class {
            throw new NotImplementedException();
        }

        public Task SendAsync(string eventType, string address, string data) {
            throw new NotImplementedException();
        }

        public Task SendAsync<T>(string eventType, string address, T @event, string correlationId) where T : class {
            throw new NotImplementedException();
        }

        public Task SendAsync<T>(T @event, string correlationId) where T : class {
            queue.Add(@event);
            Closed?.Invoke(this, null);
            return Task.CompletedTask;
        }

        public Task SendAsync(string eventType, string address, string data, string correlationId) {
            throw new NotImplementedException();
        }

        public Task SendAsync<T>(T @event, string eventType, string address, string correlationId) where T : class {
            throw new NotImplementedException();
        }

        public Task ScheduleMessageAsync<T>(T @event, DateTime scheduledEnqueueTimeUtc) where T : class {
            throw new NotImplementedException();
        }

        public Task ScheduleMessageAsync<T>(T @event, string correlationId, DateTime scheduledEnqueueTimeUtc) where T : class {
            throw new NotImplementedException();
        }

        public Task ScheduleMessageAsync<T>(T @event, string eventType, string address, string correlationId, DateTime scheduledEnqueueTimeUtc) where T : class {
            throw new NotImplementedException();
        }

        public Task ScheduleMessageAsync(string data, string eventType, string address, string correlationId, DateTime scheduledEnqueueTimeUtc) {
            throw new NotImplementedException();
        }

        public Task SendAsync<T>(T @event, string correlationId, string messageId) where T : class {
            throw new NotImplementedException();
        }

        public Task SendAsync(string eventType, string address, string data, string correlationId, string messageId) {
            throw new NotImplementedException();
        }
    }
}
