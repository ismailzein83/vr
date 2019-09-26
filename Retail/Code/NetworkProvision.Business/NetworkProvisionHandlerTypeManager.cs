using NetworkProvision.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;

namespace NetworkProvision.Business
{
    public class NetworkProvisionHandlerTypeManager
    {
        public IEnumerable<NetworkProvisionHandlerTypeExtendedSettingsConfig> GetHandlerTypeExtendedSettingsConfigs()
        {
            var templateConfigManager = new ExtensionConfigurationManager();
            return templateConfigManager.GetExtensionConfigurations<NetworkProvisionHandlerTypeExtendedSettingsConfig>(NetworkProvisionHandlerTypeExtendedSettingsConfig.EXTENSION_TYPE);
        }
    }
}
