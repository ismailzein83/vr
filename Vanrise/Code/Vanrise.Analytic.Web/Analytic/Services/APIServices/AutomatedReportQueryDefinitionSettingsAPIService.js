(function (appControllers) {

    "use strict";
    automatedReportQueryDefinitionSettingsAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_Analytic_ModuleConfig', 'SecurityService'];

    function automatedReportQueryDefinitionSettingsAPIService(BaseAPIService, UtilsService, VR_Analytic_ModuleConfig) {

        var controllerName = 'AutomatedReportQueryDefinitionSettings';

        function GetAutomatedReportTemplateConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Analytic_ModuleConfig.moduleName, controllerName, 'GetAutomatedReportTemplateConfigs'));
        }

        function GetVRAutomatedReportQueryDefinitionsInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Analytic_ModuleConfig.moduleName, controllerName, 'GetVRAutomatedReportQueryDefinitionsInfo'),filter);
        }

        function GetVRAutomatedReportQueryDefinitionSettings(vrComponentTypeId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Analytic_ModuleConfig.moduleName, controllerName, 'GetVRAutomatedReportQueryDefinitionSettings'), {
                vrComponentTypeId: vrComponentTypeId
            });
        }

        return ({
            GetAutomatedReportTemplateConfigs: GetAutomatedReportTemplateConfigs,
            GetVRAutomatedReportQueryDefinitionsInfo: GetVRAutomatedReportQueryDefinitionsInfo,
            GetVRAutomatedReportQueryDefinitionSettings: GetVRAutomatedReportQueryDefinitionSettings
        });
    }

    appControllers.service('VR_Analytic_AutomatedReportQueryDefinitionSettingsAPIService', automatedReportQueryDefinitionSettingsAPIService);

})(appControllers);