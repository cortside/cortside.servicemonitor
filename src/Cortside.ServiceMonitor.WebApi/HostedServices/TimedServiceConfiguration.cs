namespace Cortside.ServiceMonitor.WebApi.HostedServices {
    /// <summary>
    /// 
    /// </summary>
    public class TimedServiceConfiguration {
        /// <summary>
        /// 
        /// </summary>
        public bool Enabled { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int Interval { get; internal set; }
    }
}
