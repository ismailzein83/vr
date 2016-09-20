using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Routing.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;

namespace TOne.WhS.Routing.Business
{
    public class RouteRuleSettingsManager
    {
        public IEnumerable<RouteOptionSettingsGroupConfig> GetRouteOptionSettingsGroupTemplates()
        {
            ExtensionConfigurationManager manager = new ExtensionConfigurationManager();
            return manager.GetExtensionConfigurations<RouteOptionSettingsGroupConfig>(RouteOptionSettingsGroupConfig.EXTENSION_TYPE);
        }

        public IEnumerable<RouteRuleOptionOrderSettingsConfig> GetRouteOptionOrderSettingsTemplates()
        {
            ExtensionConfigurationManager manager = new ExtensionConfigurationManager();
            return manager.GetExtensionConfigurations<RouteRuleOptionOrderSettingsConfig>(RouteRuleOptionOrderSettingsConfig.EXTENSION_TYPE);
        }

        public IEnumerable<RouteRuleOptionFilterSettingsConfig> GetRouteOptionFilterSettingsTemplates()
        {
            ExtensionConfigurationManager manager = new ExtensionConfigurationManager();
            return manager.GetExtensionConfigurations<RouteRuleOptionFilterSettingsConfig>(RouteRuleOptionFilterSettingsConfig.EXTENSION_TYPE);
        }

        public IEnumerable<RouteRuleOptionPercentageSettingsConfig> GetRouteOptionPercentageSettingsTemplates()
        {
            ExtensionConfigurationManager manager = new ExtensionConfigurationManager();
            return manager.GetExtensionConfigurations<RouteRuleOptionPercentageSettingsConfig>(RouteRuleOptionPercentageSettingsConfig.EXTENSION_TYPE);
        }

        public IEnumerable<RoutingOptimizerSettingsConfig> GetRoutingOptimizerSettingsConfigs()
        {
            var extensionConfigurationManager = new ExtensionConfigurationManager();
            return extensionConfigurationManager.GetExtensionConfigurations<RoutingOptimizerSettingsConfig>(RoutingOptimizerSettingsConfig.EXTENSION_TYPE);
        }
    }
}
