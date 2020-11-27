using System.Collections.Generic;
using Cortside.Common.BootStrap;
using Cortside.ServiceMonitor.BootStrap.Installer;

namespace Cortside.ServiceMonitor.BootStrap {
    public class DefaultApplicationBootStrapper : BootStrapper {

        public DefaultApplicationBootStrapper() {
            installers = new List<IInstaller> {
                new HealthInstaller()
            };
        }
    }
}
