using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;

namespace TOne.WhS.RouteSync.Radius
{
    public class RadiusDataManagersConfigManager
    {
        public IEnumerable<RadiusDataManagerConfig> GetRadiusDataManagerExtensionConfigs()
        {
            var templateConfigManager = new ExtensionConfigurationManager();
            return templateConfigManager.GetExtensionConfigurations<RadiusDataManagerConfig>(RadiusDataManagerConfig.EXTENSION_TYPE);
        }
    }
}
