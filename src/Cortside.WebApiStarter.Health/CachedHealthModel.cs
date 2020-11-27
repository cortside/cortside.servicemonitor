using Cortside.Health.Models;

namespace Cortside.WebApiStarter.Health {
    public class CachedHealthModel : ServiceStatusModel {
        public HealthModel Data { get; set; }
        public string Content { get; internal set; }
    }
}
