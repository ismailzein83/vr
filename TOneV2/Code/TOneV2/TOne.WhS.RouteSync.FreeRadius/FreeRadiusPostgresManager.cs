using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;

namespace TOne.WhS.RouteSync.FreeRadius
{
    public class FreeRadiusPostgresManager
    {
        public IEnumerable<FreeRadiusPostgresDataManagerConfig> GetFreeRadiusPostgresDataManagerConfigs()
        {
            var templateConfigManager = new ExtensionConfigurationManager();
            return templateConfigManager.GetExtensionConfigurations<FreeRadiusPostgresDataManagerConfig>(FreeRadiusPostgresDataManagerConfig.EXTENSION_TYPE);
        }
    }
}
