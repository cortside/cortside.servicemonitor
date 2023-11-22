using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Cortside.Health;
using Cortside.Health.Checks;
using Cortside.Health.Enums;
using Cortside.Health.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;

namespace Cortside.HealthMonitor.Health {
    public class CachingHealthCheck : Check {
        private readonly RestClient client;
        private readonly JsonSerializerSettings serializerSettings;

        public CachingHealthCheck(IMemoryCache cache, ILogger<Check> logger, IAvailabilityRecorder recorder, RestClient client, IOptions<MvcNewtonsoftJsonOptions> options) : base(cache, logger, recorder) {
            this.client = client;
            this.serializerSettings = options.Value.SerializerSettings;
        }

        public override async Task<ServiceStatusModel> ExecuteAsync() {
            var request = new RestRequest(check.Value, Method.Get) {
                Timeout = (int)TimeSpan.FromSeconds(check.Timeout).TotalMilliseconds
            };

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var response = await client.ExecuteAsync(request);
            stopwatch.Stop();

            var key = "health::" + check.Name;
            if (cache.Get(key) is not CachedHealthModel model) {
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
                model.Data = JsonConvert.DeserializeObject<HealthModel>(body, serializerSettings);
            } catch (Exception ex) {
                model.Content = response.Content;
                model.StatusDetail = ex.Message;
            }

            cache.Set(key, model, DateTimeOffset.Now.AddSeconds(check.CacheDuration));
            return model;
        }
    }
}
