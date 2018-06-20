using System.Collections.Generic;
using Vanrise.Analytic.Entities;
using Vanrise.Common.Business;

namespace Vanrise.Analytic.Business
{
    public class AnalyticConfigurationManager
    {
        ExtensionConfigurationManager _templateConfigManager = new ExtensionConfigurationManager();

        public IEnumerable<HistorySearchSetting> GetAnalyticReportSettingsTemplateConfigs()
        {
            return _templateConfigManager.GetExtensionConfigurations<HistorySearchSetting>(HistorySearchSetting.EXTENSION_TYPE);
        }
        public IEnumerable<WidgetDefinitionSetting> GetWidgetsTemplateConfigs()
        {
            return _templateConfigManager.GetExtensionConfigurations<WidgetDefinitionSetting>(WidgetDefinitionSetting.EXTENSION_TYPE);
        }
        public IEnumerable<RealTimeSearchSetting> GetRealTimeReportSettingsTemplateConfigs()
        {
            return _templateConfigManager.GetExtensionConfigurations<RealTimeSearchSetting>(RealTimeSearchSetting.EXTENSION_TYPE);
        }
        public IEnumerable<RealTimeWidgetSetting> GetRealTimeWidgetsTemplateConfigs()
        {
            return _templateConfigManager.GetExtensionConfigurations<RealTimeWidgetSetting>(RealTimeWidgetSetting.EXTENSION_TYPE);
        }
        public IEnumerable<MeasureStyleRuleTemplate> GetMeasureStyleRuleTemplateConfigs()
        {
            return _templateConfigManager.GetExtensionConfigurations<MeasureStyleRuleTemplate>(MeasureStyleRuleTemplate.EXTENSION_TYPE);
        }
        public IEnumerable<AnalyticItemActionTemplate> GetAnalyticItemActionsTemplateConfigs()
        {
            return _templateConfigManager.GetExtensionConfigurations<AnalyticItemActionTemplate>(AnalyticItemActionTemplate.EXTENSION_TYPE);
        }
        public IEnumerable<AnalyticDataProviderConfig> GetAnalyticDataProviderConfigs()
        {
            return _templateConfigManager.GetExtensionConfigurations<AnalyticDataProviderConfig>(AnalyticDataProviderConfig.EXTENSION_TYPE);
        }
        public IEnumerable<VRRestAPIAnalyticQueryInterceptorConfig> GetVRRestAPIAnalyticQueryInterceptorConfigs()
        {
            return _templateConfigManager.GetExtensionConfigurations<VRRestAPIAnalyticQueryInterceptorConfig>(VRRestAPIAnalyticQueryInterceptorConfig.EXTENSION_TYPE);
        }
        public IEnumerable<DRSearchPageSubviewDefinitionSettingsConfig> GetDRSearchPageSubviewDefinitionSettingsConfigs()
        {
            return _templateConfigManager.GetExtensionConfigurations<DRSearchPageSubviewDefinitionSettingsConfig>(DRSearchPageSubviewDefinitionSettingsConfig.EXTENSION_TYPE);
        }

        public IEnumerable<MeasureExternalSourceSettingConfig> GetMeasureExternalSourceExtendedSettingConfigs()
        {
            return _templateConfigManager.GetExtensionConfigurations<MeasureExternalSourceSettingConfig>(MeasureExternalSourceSettingConfig.EXTENSION_TYPE);
        }
        public IEnumerable<DimensionMappingRuleSettingConfig> GetDimensionMappingRuleSettingConfigs()
        {
            return _templateConfigManager.GetExtensionConfigurations<DimensionMappingRuleSettingConfig>(DimensionMappingRuleSettingConfig.EXTENSION_TYPE);
        }
        public IEnumerable<MeasureMappingRuleSettingConfig> GetMeasureMappingRuleSettingConfigs()
        {
            return _templateConfigManager.GetExtensionConfigurations<MeasureMappingRuleSettingConfig>(MeasureMappingRuleSettingConfig.EXTENSION_TYPE);
        }
    }
}
