using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace SecureTokenService
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        public ProjectInstaller()
        {
            var processInstaller = new ServiceProcessInstaller
            {
                Account = ServiceAccount.LocalSystem // Run under LocalSystem account
            };

            // Create and configure the service installer
            var serviceInstaller = new ServiceInstaller
            {
                ServiceName = "TokenVaultService", // Name of your service
                DisplayName = "Test Token", // Display name in the Services Manager
                Description = "Vault token caching service for offline authentication", // Description of your service
                StartType = ServiceStartMode.Automatic // Start automatically
            };

            // Add installers to the Installers collection
            Installers.Add(processInstaller);
            Installers.Add(serviceInstaller);
        }
    }
}

