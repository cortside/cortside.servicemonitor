using System.Collections.Generic;

namespace Cortside.ServiceMonitor.WebApi.HostedServices {

    /// <summary>
    /// config
    /// </summary>
    public class HealthCheckServiceConfiguration : TimedServiceConfiguration {
        public bool Enabled { get; set; }
        public int CheckInterval { get; set; }
        public int CacheDuration { get; set; }
        public List<Check> Checks { get; set; }
    }


    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class Check {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
    }
}
