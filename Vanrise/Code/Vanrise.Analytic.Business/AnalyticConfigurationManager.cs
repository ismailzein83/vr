using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public IEnumerable<TemplateConfig> GetWidgetsTemplateConfigs()
        {
            var templateConfigManager = new TemplateConfigManager();
            return templateConfigManager.GetTemplateConfigurations(Constants.AnalyticWidgetsConfigType);
        }
    }
}
