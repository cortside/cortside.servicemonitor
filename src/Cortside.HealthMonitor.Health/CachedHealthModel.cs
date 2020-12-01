using Cortside.Health.Models;

namespace Cortside.HealthMonitor.Health {
    public class CachedHealthModel : ServiceStatusModel {
        public HealthModel Data { get; set; }
        public string Content { get; internal set; }
    }
}
