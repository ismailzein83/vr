(function (appControllers) {

    "use strict";
    automatedReportHandlerAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_Analytic_ModuleConfig'];

    function automatedReportHandlerAPIService(BaseAPIService, UtilsService, VR_Analytic_ModuleConfig) {

        var controllerName = 'AutomatedReportHandler';

        function GetFileGeneratorTemplateConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Analytic_ModuleConfig.moduleName, controllerName, 'GetFileGeneratorTemplateConfigs'));
        }
         
        return ({
            GetFileGeneratorTemplateConfigs: GetFileGeneratorTemplateConfigs,
        });
    }

    appControllers.service('VRAnalytic_AutomatedReportHandlerAPIService', automatedReportHandlerAPIService);

})(appControllers);