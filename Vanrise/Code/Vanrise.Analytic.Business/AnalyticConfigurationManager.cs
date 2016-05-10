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
        public IEnumerable<HistorySearchSetting> GetAnalyticReportSettingsTemplateConfigs()
        {
            var templateConfigManager = new ExtensionConfigurationManager();
            return templateConfigManager.GetExtensionConfigurations<HistorySearchSetting>(Constants.AnalyticHistoryReportSettingsConfigType);
        }
        public IEnumerable<WidgetDefinitionSetting> GetWidgetsTemplateConfigs()
        {
            var templateConfigManager = new ExtensionConfigurationManager();
            return templateConfigManager.GetExtensionConfigurations<WidgetDefinitionSetting>(Constants.AnalyticWidgetsConfigType);
        }

        public IEnumerable<RealTimeSearchSetting> GetRealTimeReportSettingsTemplateConfigs()
        {
            var templateConfigManager = new ExtensionConfigurationManager();
            return templateConfigManager.GetExtensionConfigurations<RealTimeSearchSetting>(Constants.RealTimeSearchConfigType);
        }
        public IEnumerable<RealTimeWidgetSetting> GetRealTimeWidgetsTemplateConfigs()
        {
            var templateConfigManager = new ExtensionConfigurationManager();
            return templateConfigManager.GetExtensionConfigurations<RealTimeWidgetSetting>(Constants.RealTimeWidgetConfigType);
        } 
    }
}
 