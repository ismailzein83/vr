using System.Collections.Generic;
using TOne.WhS.Routing.Entities;
using Vanrise.Common.Business;

namespace TOne.WhS.Routing.Business
{
    public class RoutingConfigurationManager
    {
        ExtensionConfigurationManager _templateConfigManager = new ExtensionConfigurationManager();

        public IEnumerable<RoutingExcludedDestinationsConfig> GetRoutingExcludedDestinationsTemplateConfigs()
        {
            return _templateConfigManager.GetExtensionConfigurations<RoutingExcludedDestinationsConfig>(RoutingExcludedDestinationsConfig.EXTENSION_TYPE);
        }
    }
}