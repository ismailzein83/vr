using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Entities;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace Vanrise.Analytic.Business
{
    public class AnalyticConfigurationManager
    {
        public IEnumerable<TemplateConfig> GetAnalyticReportSettingsTemplateConfigs()
        {
            var templateConfigManager = new TemplateConfigManager();
            return templateConfigManager.GetTemplateConfigurations(Constants.AnalyticReportSettingsConfigType);
        }
        public IEnumerable<WidgetDefinitionSetting> GetWidgetsTemplateConfigs()
        {
            var templateConfigManager = new ExtensionConfigurationManager();
            return templateConfigManager.GetExtensionConfigurations<WidgetDefinitionSetting>(Constants.AnalyticWidgetsConfigType);
        }
    }
}
 