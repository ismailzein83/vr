using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;

namespace TOne.WhS.RouteSync.FreeRadius
{
    public class FreeRadiusManager
    {
        public IEnumerable<FreeRadiusDataManagerConfig> GetFreeRadiusDataManagerConfigs()
        {
            var templateConfigManager = new ExtensionConfigurationManager();
            return templateConfigManager.GetExtensionConfigurations<FreeRadiusDataManagerConfig>(FreeRadiusDataManagerConfig.EXTENSION_TYPE);
        }
    }
}
