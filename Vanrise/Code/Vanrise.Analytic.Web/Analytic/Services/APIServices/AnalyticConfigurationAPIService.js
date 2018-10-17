﻿(function (appControllers) {

    "use strict";

    AnalyticConfigurationAPIService.$inject = ['BaseAPIService', 'VR_Analytic_ModuleConfig', 'UtilsService', 'SecurityService'];

    function AnalyticConfigurationAPIService(BaseAPIService, VR_Analytic_ModuleConfig, UtilsService, SecurityService) {

        var controllerName = 'AnalyticConfiguration';

        function GetAnalyticReportSettingsTemplateConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Analytic_ModuleConfig.moduleName, controllerName, "GetAnalyticReportSettingsTemplateConfigs"));
        }

        function GetWidgetsTemplateConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Analytic_ModuleConfig.moduleName, controllerName, "GetWidgetsTemplateConfigs"));
        }

        function GetRealTimeReportSettingsTemplateConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Analytic_ModuleConfig.moduleName, controllerName, "GetRealTimeReportSettingsTemplateConfigs"));
        }

        function GetRealTimeWidgetsTemplateConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Analytic_ModuleConfig.moduleName, controllerName, "GetRealTimeWidgetsTemplateConfigs"));
        }

        function GetMeasureStyleRuleTemplateConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Analytic_ModuleConfig.moduleName, controllerName, "GetMeasureStyleRuleTemplateConfigs"));
        }

        function GetAnalyticDataProviderConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Analytic_ModuleConfig.moduleName, controllerName, "GetAnalyticDataProviderConfigs"));
        }

        function GetVRRestAPIAnalyticQueryInterceptorConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Analytic_ModuleConfig.moduleName, controllerName, "GetVRRestAPIAnalyticQueryInterceptorConfigs"));
        }

        function GetDRSearchPageSubviewDefinitionSettingsConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Analytic_ModuleConfig.moduleName, controllerName, "GetDRSearchPageSubviewDefinitionSettingsConfigs"));
        }

        function GetMeasureExternalSourceExtendedSettingConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Analytic_ModuleConfig.moduleName, controllerName, "GetMeasureExternalSourceExtendedSettingConfigs"));
        }

        function GetDimensionMappingRuleSettingConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Analytic_ModuleConfig.moduleName, controllerName, "GetDimensionMappingRuleSettingConfigs"));
        }

        function GetMeasureMappingRuleSettingConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Analytic_ModuleConfig.moduleName, controllerName, "GetMeasureMappingRuleSettingConfigs"));
        }

        function GetDAProfCalcAlertRuleFilterDefinitionConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Analytic_ModuleConfig.moduleName, controllerName, "GetDAProfCalcAlertRuleFilterDefinitionConfigs"));
        }

        function GetDAProfCalcAlertRuleFilterConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Analytic_ModuleConfig.moduleName, controllerName, "GetDAProfCalcAlertRuleFilterConfigs"));
        }

        return {
            GetAnalyticReportSettingsTemplateConfigs: GetAnalyticReportSettingsTemplateConfigs,
            GetWidgetsTemplateConfigs: GetWidgetsTemplateConfigs,
            GetRealTimeReportSettingsTemplateConfigs: GetRealTimeReportSettingsTemplateConfigs,
            GetRealTimeWidgetsTemplateConfigs: GetRealTimeWidgetsTemplateConfigs,
            GetMeasureStyleRuleTemplateConfigs: GetMeasureStyleRuleTemplateConfigs,
            GetAnalyticDataProviderConfigs: GetAnalyticDataProviderConfigs,
            GetVRRestAPIAnalyticQueryInterceptorConfigs: GetVRRestAPIAnalyticQueryInterceptorConfigs,
            GetDRSearchPageSubviewDefinitionSettingsConfigs: GetDRSearchPageSubviewDefinitionSettingsConfigs,
            GetMeasureExternalSourceExtendedSettingConfigs: GetMeasureExternalSourceExtendedSettingConfigs,
            GetDimensionMappingRuleSettingConfigs: GetDimensionMappingRuleSettingConfigs,
            GetMeasureMappingRuleSettingConfigs: GetMeasureMappingRuleSettingConfigs,
            GetDAProfCalcAlertRuleFilterDefinitionConfigs: GetDAProfCalcAlertRuleFilterDefinitionConfigs,
            GetDAProfCalcAlertRuleFilterConfigs: GetDAProfCalcAlertRuleFilterConfigs
        };
    }

    appControllers.service('VR_Analytic_AnalyticConfigurationAPIService', AnalyticConfigurationAPIService);

})(appControllers);