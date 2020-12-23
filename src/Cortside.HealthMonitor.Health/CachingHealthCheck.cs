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
        public CachingHealthCheck(IMemoryCache cache, ILogger<Check> logger, IAvailabilityRecorder recorder) : base(cache, logger, recorder) { }

        public override async Task<ServiceStatusModel> ExecuteAsync() {
            var swtotal = new Stopwatch();
            swtotal.Start();

            var client = new RestClient();
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

            var swread = new Stopwatch();
            swread.Start();
            try {
                var body = response.Content.Replace(Environment.NewLine, "");
                model.Data = JsonConvert.DeserializeObject<HealthModel>(body);
            } catch {
                model.Content = response.Content;
            }
            swread.Stop();

            cache.Set(key, model, DateTimeOffset.Now.AddSeconds(check.CacheDuration));

            swtotal.Stop();
            if (swtotal.ElapsedMilliseconds > request.Timeout) {
                logger.LogWarning($"Excution took {swtotal.ElapsedMilliseconds}ms with {stopwatch.ElapsedMilliseconds}ms in request time, {swread.ElapsedMilliseconds}ms in response read and deserialization leaving {swtotal.ElapsedMilliseconds - swread.ElapsedMilliseconds - stopwatch.ElapsedMilliseconds}ms in everything else");
            }

            return model;
        }
    }
}
