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
            return templateConfigManager.GetExtensionConfigurations<HistorySearchSetting>(HistorySearchSetting.EXTENSION_TYPE);
        }
        public IEnumerable<WidgetDefinitionSetting> GetWidgetsTemplateConfigs()
        {
            var templateConfigManager = new ExtensionConfigurationManager();
            return templateConfigManager.GetExtensionConfigurations<WidgetDefinitionSetting>(WidgetDefinitionSetting.EXTENSION_TYPE);
        }

        public IEnumerable<RealTimeSearchSetting> GetRealTimeReportSettingsTemplateConfigs()
        {
            var templateConfigManager = new ExtensionConfigurationManager();
            return templateConfigManager.GetExtensionConfigurations<RealTimeSearchSetting>(RealTimeSearchSetting.EXTENSION_TYPE);
        }
        public IEnumerable<RealTimeWidgetSetting> GetRealTimeWidgetsTemplateConfigs()
        {
            var templateConfigManager = new ExtensionConfigurationManager();
            return templateConfigManager.GetExtensionConfigurations<RealTimeWidgetSetting>(RealTimeWidgetSetting.EXTENSION_TYPE);
        }
        public IEnumerable<MeasureStyleRuleTemplate> GetMeasureStyleRuleTemplateConfigs()
        {
            var templateConfigManager = new ExtensionConfigurationManager();
            return templateConfigManager.GetExtensionConfigurations<MeasureStyleRuleTemplate>(MeasureStyleRuleTemplate.EXTENSION_TYPE);
        }
        public IEnumerable<AnalyticItemActionTemplate> GetAnalyticItemActionsTemplateConfigs()
        {
            var templateConfigManager = new ExtensionConfigurationManager();
            return templateConfigManager.GetExtensionConfigurations<AnalyticItemActionTemplate>(AnalyticItemActionTemplate.EXTENSION_TYPE);
        } 
    }
}
 