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

        function GetAutomatedReportDataSchema(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Analytic_ModuleConfig.moduleName, controllerName, 'GetAutomatedReportDataSchema'), input);
        }

        function ValidateQueryAndHandlerSettings(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Analytic_ModuleConfig.moduleName, controllerName, 'ValidateQueryAndHandlerSettings'), input);
        }

        return ({
            GetAutomatedReportTemplateConfigs: GetAutomatedReportTemplateConfigs,
            GetVRAutomatedReportQueryDefinitionsInfo: GetVRAutomatedReportQueryDefinitionsInfo,
            GetVRAutomatedReportQueryDefinitionSettings: GetVRAutomatedReportQueryDefinitionSettings,
            GetAutomatedReportDataSchema: GetAutomatedReportDataSchema,
            ValidateQueryAndHandlerSettings: ValidateQueryAndHandlerSettings
        });
    }

    appControllers.service('VR_Analytic_AutomatedReportQueryDefinitionSettingsAPIService', automatedReportQueryDefinitionSettingsAPIService);

})(appControllers);