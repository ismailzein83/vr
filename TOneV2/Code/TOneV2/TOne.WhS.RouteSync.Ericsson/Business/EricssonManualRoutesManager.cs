using System.Collections.Generic;
using TOne.WhS.RouteSync.Ericsson.Entities;
using Vanrise.Common.Business;

namespace TOne.WhS.RouteSync.Ericsson.Business
{
    public class EricssonManualRoutesManager
    {
        public IEnumerable<EricssonManualRouteActionConfig> GetManualRouteActionTypeExtensionConfigs()
        {
            ExtensionConfigurationManager manager = new ExtensionConfigurationManager();
            return manager.GetExtensionConfigurations<EricssonManualRouteActionConfig>(EricssonManualRouteActionConfig.EXTENSION_TYPE);
        }

        public IEnumerable<EricssonManualRouteDestinationsConfig> GetManualRouteDestinationsTypeExtensionConfigs()
        {
            ExtensionConfigurationManager manager = new ExtensionConfigurationManager();
            return manager.GetExtensionConfigurations<EricssonManualRouteDestinationsConfig>(EricssonManualRouteDestinationsConfig.EXTENSION_TYPE);
        }

        public IEnumerable<EricssonManualRouteOriginationsConfig> GetManualRouteOriginationsTypeExtensionConfigs()
        {
            ExtensionConfigurationManager manager = new ExtensionConfigurationManager();
            return manager.GetExtensionConfigurations<EricssonManualRouteOriginationsConfig>(EricssonManualRouteOriginationsConfig.EXTENSION_TYPE);
        }

        public IEnumerable<EricssonSpecialRoutingSettingConfig> GetSpecialRoutingTypeExtensionConfigs()
        {
            ExtensionConfigurationManager manager = new ExtensionConfigurationManager();
            return manager.GetExtensionConfigurations<EricssonSpecialRoutingSettingConfig>(EricssonSpecialRoutingSettingConfig.EXTENSION_TYPE);
        }
    }
}
