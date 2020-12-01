using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Cortside.HealthMonitor.DomainService;
using Cortside.HealthMonitor.WebApi.Models.Responses;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PolicyServer.Runtime.Client;
using RestSharp;

namespace Cortside.HealthMonitor.WebApi.HostedServices {




    /// <summary>
    /// Background service for health check
    /// </summary>
    public class HealthCheckHostedService : BackgroundService {


        // The Application Insights Instrumentation Key can be changed by going to the overview page of your Function App, selecting configuration, and changing the value of the APPINSIGHTS_INSTRUMENTATIONKEY Application setting.
        // DO NOT replace the code below with your instrumentation key, the key's value is pulled from the environment variable/application setting key/value pair.
        private static readonly string instrumentationKey = "c42b7a92-416b-4163-9835-bc3633440bb6";

        //[CONFIGURATION_REQUIRED]
        // If your resource is in a region like Azure Government or Azure China, change the endpoint address accordingly.
        // Visit https://docs.microsoft.com/azure/azure-monitor/app/custom-endpoints#regions-that-require-endpoint-modification for more details.
        private const string EndpointAddress = "https://dc.services.visualstudio.com/v2/track";

        private static readonly TelemetryConfiguration telemetryConfiguration = new TelemetryConfiguration(instrumentationKey, new InMemoryChannel { EndpointAddress = EndpointAddress });
        private static readonly TelemetryClient telemetryClient = new TelemetryClient(telemetryConfiguration);


        private readonly ILogger logger;
        private readonly IConfiguration configuration;
        private readonly IHealthService healthService;
        private readonly IPolicyServerRuntimeClient policyServerClient;
        private IMemoryCache cache;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="configuration"></param>
        /// <param name="cache"></param>
        /// <param name="healthService"></param>
        /// <param name="policyServerRuntimeClient"></param>
        public HealthCheckHostedService(ILogger<HealthCheckHostedService> logger, IConfiguration configuration, IMemoryCache cache, IHealthService healthService, IPolicyServerRuntimeClient policyServerRuntimeClient) {
            this.logger = logger;
            this.configuration = configuration;
            this.cache = cache;
            this.healthService = healthService;
            this.policyServerClient = policyServerRuntimeClient;
        }

        /// <summary>
        /// Process to fire off health check
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        protected async override Task ExecuteAsync(CancellationToken stoppingToken) {
            if (!configuration.GetSection("HealthCheckHostedService").GetValue<bool>("Enable")) {
                logger.LogInformation("HealthCheckHostedService is disabled");
            } else {
                while (!stoppingToken.IsCancellationRequested) {
                    await Task.Delay(5000, stoppingToken);
                    try {

                        var policyServerTask = GetPolicyHealthCheck();
                        var idsTask = GetIdentityServerHealthCheck();
                        var health = CacheHealth();
                        await Task.WhenAll(policyServerTask, idsTask, health);
                    } catch (Exception ex) {
                        logger.LogError(ex, "Unhandled exception in execute of TimedHostedService");
                    }
                }
            }
        }

        private async Task GetPolicyHealthCheck() {
            logger.LogInformation("Retrieving Policy Server Status.");

            var item = cache.Get<ServiceStatusModel>(HealthCheckConstants.CACHE_KEY_POLICYSERVER);
            var age = item != null ? (DateTime.UtcNow - item.Timestamp).TotalSeconds : int.MaxValue;
            if (age >= 30) {
                var stopwatch = new Stopwatch();
                stopwatch.Start();

                ServiceStatusModel serviceStatusModel;
                try {
                    var url = configuration.GetSection("PolicyServer").GetValue<string>("PolicyServerUrl");
                    var client = new RestClient(url);
                    var request = new RestRequest("health", Method.GET);
                    request.Timeout = 5000;
                    var response = await client.ExecuteTaskAsync(request);
                    serviceStatusModel = new ServiceStatusModel() {
                        Healthy = response.IsSuccessful,
                        Status = response.IsSuccessful ? ServiceStatus.Ok : ServiceStatus.Failure,
                        StatusDetail = response.IsSuccessful ? "Successful" : response.ErrorMessage,
                        Timestamp = DateTime.UtcNow
                    };
                } catch {
                    serviceStatusModel = new ServiceStatusModel() {
                        Healthy = false,
                        Timestamp = DateTime.UtcNow
                    };
                }
                // Store it in cache
                cache.Set(HealthCheckConstants.CACHE_KEY_POLICYSERVER, serviceStatusModel, DateTimeOffset.Now.AddSeconds(45));

                stopwatch.Stop();
                RecordTelemetry("policyserver", stopwatch.Elapsed, serviceStatusModel.Healthy, JsonConvert.SerializeObject(serviceStatusModel));
            }
        }

        private async Task GetIdentityServerHealthCheck() {
            logger.LogInformation("Retrieving Identity Server Status.");

            var item = cache.Get<ServiceStatusModel>(HealthCheckConstants.CACHE_KEY_IDS);
            var age = item != null ? (DateTime.UtcNow - item.Timestamp).TotalSeconds : int.MaxValue;
            if (age >= 30) {
                var stopwatch = new Stopwatch();
                stopwatch.Start();

                ServiceStatusModel serviceStatusModel;
                try {
                    var identityServerTask = await healthService.GetIdentityServerHealthCheck();
                    serviceStatusModel = new ServiceStatusModel {
                        Healthy = identityServerTask.Healthy,
                        Status = identityServerTask.Healthy ? ServiceStatus.Ok : ServiceStatus.Failure,
                        Timestamp = DateTime.UtcNow
                    };
                } catch {
                    serviceStatusModel = new ServiceStatusModel() {
                        Healthy = false,
                        Timestamp = DateTime.UtcNow
                    };
                }
                // Store it in cache
                cache.Set(HealthCheckConstants.CACHE_KEY_IDS, serviceStatusModel, DateTimeOffset.Now.AddSeconds(45));

                stopwatch.Stop();
                RecordTelemetry("identityserver", stopwatch.Elapsed, serviceStatusModel.Healthy, JsonConvert.SerializeObject(serviceStatusModel));
            }
        }

        private async Task CacheHealth() {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            HealthModel response;
            var runningTime = TimeSpan.MaxValue;
            try {
                Process currentProcess = Process.GetCurrentProcess();
                if (currentProcess != null) {
                    runningTime = DateTime.UtcNow.Subtract(currentProcess.StartTime);
                }
            } catch (Exception) {
                // ignore this, not critical
            }

            var dbstatus = healthService.GetDatabaseStatus();
            var db = new ServiceStatusModel {
                Healthy = dbstatus,
                Status = dbstatus ? ServiceStatus.Ok : ServiceStatus.Failure,
                Timestamp = DateTime.UtcNow
            };

            var build = configuration.GetSection("Build").Get<BuildModel>();
            var policyServerStatus = cache.Get<ServiceStatusModel>(HealthCheckConstants.CACHE_KEY_POLICYSERVER) ?? new ServiceStatusModel();
            var idsStatus = cache.Get<ServiceStatusModel>(HealthCheckConstants.CACHE_KEY_IDS) ?? new ServiceStatusModel();

            response = await Task.Run(() => new HealthModel() {
                Build = build,
                Checks = new ChecksModel() {
                    Database = new ServiceStatusModel() {
                        Healthy = dbstatus,
                        Status = dbstatus ? ServiceStatus.Ok : ServiceStatus.Failure,
                        Timestamp = DateTime.UtcNow
                    },
                    PolicyServer = policyServerStatus,
                    IdentityServer = idsStatus
                },
                Timestamp = DateTime.UtcNow,
                Uptime = runningTime
            });

            response.Service = "HealthMonitor-api";
            var status = db.Status;
            var healthy = db.Healthy;
            if (healthy && (!response.Checks.Database.Healthy
                            || !response.Checks.PolicyServer.Healthy
                            || !response.Checks.IdentityServer.Healthy)) {
                status = ServiceStatus.Degraded;
            }

            response.Status = status;
            response.Healthy = healthy;
            response.Cached = true;

            stopwatch.Stop();
            RecordTelemetry(response.Service, stopwatch.Elapsed, healthy, JsonConvert.SerializeObject(response));

            if (status != ServiceStatus.Ok) {
                logger.LogError($"Health Check Response {JsonConvert.SerializeObject(response)}");
            }
            cache.Set(HealthCheckConstants.CACHE_KEY, response, DateTimeOffset.Now.AddSeconds(30));
        }

        private void RecordTelemetry(string service, TimeSpan duration, bool healthy, string message) {
            // [CONFIGURATION_REQUIRED] provide {testName} accordingly for your test function
            string testName = service;

            // REGION_NAME is a default environment variable that comes with App Service
            string location = Environment.MachineName;

            logger.LogInformation($"Executing availability test run for {testName} at: {DateTime.Now}");
            string operationId = Guid.NewGuid().ToString("N");

            var availability = new AvailabilityTelemetry {
                Id = operationId,
                Name = testName,
                RunLocation = location,
                Success = false
            };

            if (healthy) {
                availability.Success = true;
            } else {
                availability.Message = message;

                var exceptionTelemetry = new ExceptionTelemetry();
                exceptionTelemetry.Context.Operation.Id = operationId;
                exceptionTelemetry.Properties.Add("TestName", testName);
                exceptionTelemetry.Properties.Add("TestLocation", location);
                telemetryClient.TrackException(exceptionTelemetry);
            }

            availability.Duration = duration;
            availability.Timestamp = DateTimeOffset.UtcNow;

            telemetryClient.TrackAvailability(availability);
            // call flush to ensure telemetry is sent
            telemetryClient.Flush();
        }
    }
}
