using System.Collections.Generic;
using Cortside.HealthMonitor.Health;

namespace Cortside.HealthMonitor.WebApi.Models {
    /// <summary>
    /// Settings
    /// </summary>
    public class ServicesModel {
        /// <summary>
        /// Monitored services
        /// </summary>
        public Dictionary<string, CachedHealthModel> Services { get; set; }
    }
}
