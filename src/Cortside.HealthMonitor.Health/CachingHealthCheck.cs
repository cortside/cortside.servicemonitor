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

namespace Cortside.HealthMonitor.Health {
    public class CachingHealthCheck : Check {
        private readonly IRestClient client;

        public CachingHealthCheck(IMemoryCache cache, ILogger<Check> logger, IAvailabilityRecorder recorder, IRestClient client) : base(cache, logger, recorder) {
            this.client = client;
        }

        public override async Task<ServiceStatusModel> ExecuteAsync() {
            var request = new RestRequest(check.Value, Method.GET);
            request.Timeout = (int)TimeSpan.FromSeconds(check.Timeout).TotalMilliseconds;

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var response = await client.ExecuteTaskAsync(request);
            stopwatch.Stop();

            var key = "health::" + check.Name;
            CachedHealthModel model = cache.Get(key) as CachedHealthModel;
            if (model == null) {
                model = new CachedHealthModel() { Availability = new Availability() };
            }

            model.Healthy = response.IsSuccessful;
            model.Status = response.IsSuccessful ? ServiceStatus.Ok : ServiceStatus.Failure;
            model.StatusDetail = response.IsSuccessful ? "Successful" : response.ErrorMessage;
            model.Timestamp = DateTime.UtcNow;
            model.Required = check.Required;
            model.Availability.UpdateStatistics(model.Healthy, stopwatch.ElapsedMilliseconds);

            var body = response.Content.Replace(Environment.NewLine, "");
            try {
                model.Data = JsonConvert.DeserializeObject<HealthModel>(body);
            } catch (Exception ex) {
                model.Content = response.Content;
                model.StatusDetail = ex.Message;
            }

            cache.Set(key, model, DateTimeOffset.Now.AddSeconds(check.CacheDuration));
            return model;
        }
    }
}
