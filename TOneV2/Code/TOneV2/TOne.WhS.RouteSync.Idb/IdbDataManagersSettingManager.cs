using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;

namespace TOne.WhS.RouteSync.Idb
{
    public class IdbDataManagersConfigManager 
    {
        public IEnumerable<IdbDataManagerConfig> GetIdbDataManagerExtensionConfigs()
        {
            var templateConfigManager = new ExtensionConfigurationManager();
            return templateConfigManager.GetExtensionConfigurations<IdbDataManagerConfig>(IdbDataManagerConfig.EXTENSION_TYPE);
        }
    }
}
