using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Cortside.Health;
using Cortside.Health.Checks;
using Cortside.Health.Enums;
using Cortside.Health.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RestSharp;

namespace Cortside.ServiceMonitor.Health {
    public class CachingHealthCheck : Check {
        public CachingHealthCheck(IMemoryCache cache, ILogger<Check> logger, IAvailabilityRecorder recorder) : base(cache, logger, recorder) { }

        public override async Task<ServiceStatusModel> ExecuteAsync() {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var client = new RestClient();
            var request = new RestRequest(check.Value, Method.GET);
            request.Timeout = (int)TimeSpan.FromSeconds(check.Timeout).TotalMilliseconds;

            var response = await client.ExecuteTaskAsync(request);

            var key = "health::" + check.Name;
            CachedHealthModel model = cache.Get(key) as CachedHealthModel;
            if (model == null) {
                model = new CachedHealthModel() { Availability = new Availability() };
            }

            stopwatch.Stop();

            model.Healthy = response.IsSuccessful;
            model.Status = response.IsSuccessful ? ServiceStatus.Ok : ServiceStatus.Failure;
            model.StatusDetail = response.IsSuccessful ? "Successful" : response.ErrorMessage;
            model.Timestamp = DateTime.UtcNow;
            model.Required = check.Required;
            model.Availability.UpdateStatistics(model.Healthy, stopwatch.ElapsedMilliseconds);

            try {
                model.Data = JsonConvert.DeserializeObject<HealthModel>(response.Content);
            } catch {
                model.Content = response.Content;
            }

            cache.Set(key, model, DateTimeOffset.Now.AddSeconds(check.CacheDuration));
            return model;
        }
    }
}
