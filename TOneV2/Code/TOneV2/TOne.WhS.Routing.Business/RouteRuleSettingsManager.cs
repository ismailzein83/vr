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
    }
}
