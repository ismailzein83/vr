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
        public List<Vanrise.Entities.TemplateConfig> GetRouteOptionSettingsGroupTemplates()
        {
            TemplateConfigManager manager = new TemplateConfigManager();
            return manager.GetTemplateConfigurations(Constants.RouteOptionSettingsGroup);
        }

        public List<Vanrise.Entities.TemplateConfig> GetRouteOptionOrderSettingsTemplates()
        {
            TemplateConfigManager manager = new TemplateConfigManager();
            return manager.GetTemplateConfigurations(Constants.RouteRuleOptionOrderSettings);
        }

        public List<Vanrise.Entities.TemplateConfig> GetRouteOptionFilterSettingsTemplates()
        {
            TemplateConfigManager manager = new TemplateConfigManager();
            return manager.GetTemplateConfigurations(Constants.RouteRuleOptionFilterSettings);
        }

        public List<Vanrise.Entities.TemplateConfig> GetRouteOptionPercentageSettingsTemplates()
        {
            TemplateConfigManager manager = new TemplateConfigManager();
            return manager.GetTemplateConfigurations(Constants.RouteRuleOptionPercentageSettings);
        }

        public IEnumerable<RoutingOptimizerSettingsConfig> GetRoutingOptimizerSettingsConfigs()
        {
            var extensionConfigurationManager = new ExtensionConfigurationManager();
            return extensionConfigurationManager.GetExtensionConfigurations<RoutingOptimizerSettingsConfig>(RoutingOptimizerSettingsConfig.EXTENSION_TYPE);
        }
    }
}
