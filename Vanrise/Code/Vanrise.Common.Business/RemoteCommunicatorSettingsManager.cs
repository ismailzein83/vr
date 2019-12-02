using System.Collections.Generic;
using System.Linq;

namespace Vanrise.Common.Business
{
    public class RemoteCommunicatorSettingsManager
    {
        public IEnumerable<RemoteCommunicatorSettingsConfig> GetRemoteCommunicatorSettingsConfigs()
        {
            ExtensionConfigurationManager manager = new ExtensionConfigurationManager();
            return manager.GetExtensionConfigurations<RemoteCommunicatorSettingsConfig>(RemoteCommunicatorSettingsConfig.EXTENSION_TYPE).OrderByDescending(x => x.Name);
        }
    }
}
