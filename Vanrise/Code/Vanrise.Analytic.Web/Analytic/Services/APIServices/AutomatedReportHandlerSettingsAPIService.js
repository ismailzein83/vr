(function (appControllers) {

    "use strict";
    automatedReportHandlerSettingsAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_Analytic_ModuleConfig', 'SecurityService'];

    function automatedReportHandlerSettingsAPIService(BaseAPIService, UtilsService, VR_Analytic_ModuleConfig) {

        var controllerName = 'AutomatedReportHandlerSettings';

        function GetAutomatedReportHandlerTemplateConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Analytic_ModuleConfig.moduleName, controllerName, 'GetAutomatedReportHandlerTemplateConfigs'));
        }

        return ({
            GetAutomatedReportHandlerTemplateConfigs: GetAutomatedReportHandlerTemplateConfigs,

        });
    }

    appControllers.service('VR_Analytic_AutomatedReportHandlerSettingsAPIService', automatedReportHandlerSettingsAPIService);

})(appControllers);