using System.Collections.Generic;
using TOne.WhS.RouteSync.Cataleya.Entities;
using Vanrise.Common.Business;

namespace TOne.WhS.RouteSync.Cataleya.Business
{
    public class CataleyaManager
    {
        public IEnumerable<CataleyaDataManagerConfig> GetCataleyaDataManagerConfigs()
        {
            var templateConfigManager = new ExtensionConfigurationManager();
            return templateConfigManager.GetExtensionConfigurations<CataleyaDataManagerConfig>(CataleyaDataManagerConfig.EXTENSION_TYPE);
        }
    }
}