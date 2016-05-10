(function (appControllers) {

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
        function GetRealTimeReportSettingsTemplateConfigs()
        {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Analytic_ModuleConfig.moduleName, controllerName, "GetRealTimeReportSettingsTemplateConfigs"));
        }
        function GetRealTimeWidgetsTemplateConfigs()
        {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Analytic_ModuleConfig.moduleName, controllerName, "GetRealTimeWidgetsTemplateConfigs"));
        }
        return ({
            GetAnalyticReportSettingsTemplateConfigs: GetAnalyticReportSettingsTemplateConfigs,
            GetWidgetsTemplateConfigs: GetWidgetsTemplateConfigs,
            GetRealTimeReportSettingsTemplateConfigs: GetRealTimeReportSettingsTemplateConfigs,
            GetRealTimeWidgetsTemplateConfigs: GetRealTimeWidgetsTemplateConfigs
        });
    }

    appControllers.service('VR_Analytic_AnalyticConfigurationAPIService', AnalyticConfigurationAPIService);

})(appControllers);