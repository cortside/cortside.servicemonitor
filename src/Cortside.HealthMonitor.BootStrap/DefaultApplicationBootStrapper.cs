using System.Collections.Generic;
using Cortside.Common.BootStrap;
using Cortside.HealthMonitor.BootStrap.Installer;

namespace Cortside.HealthMonitor.BootStrap {
    public class DefaultApplicationBootStrapper : BootStrapper {
        public DefaultApplicationBootStrapper() {
            installers = new List<IInstaller> {
                new RestSharpInstaller()
            };
        }
    }
}
