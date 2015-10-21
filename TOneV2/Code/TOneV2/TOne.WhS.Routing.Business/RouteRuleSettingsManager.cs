using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

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
    }
}
