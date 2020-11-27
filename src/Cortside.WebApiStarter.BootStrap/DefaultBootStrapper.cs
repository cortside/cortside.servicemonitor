using System.Collections.Generic;
using Cortside.Common.BootStrap;
using Cortside.WebApiStarter.BootStrap.Installer;

namespace Cortside.WebApiStarter.BootStrap {
    public class DefaultApplicationBootStrapper : BootStrapper {

        public DefaultApplicationBootStrapper() {
            installers = new List<IInstaller> {
                new DomainEventInstaller(),
                new ExampleHostedServiceInstaller(),
                new HealthInstaller(),
                new DbContextInstaller(),
                new DomainInstaller()
            };
        }
    }
}
