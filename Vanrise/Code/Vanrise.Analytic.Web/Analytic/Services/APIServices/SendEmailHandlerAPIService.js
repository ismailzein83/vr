(function (appControllers) {

    "use strict";
    sendEmailHandlerAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_Analytic_ModuleConfig', 'SecurityService'];

    function sendEmailHandlerAPIService(BaseAPIService, UtilsService, VR_Analytic_ModuleConfig) {

        var controllerName = 'SendEmailHandler';

        function GetFileGeneratorTemplateConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Analytic_ModuleConfig.moduleName, controllerName, 'GetFileGeneratorTemplateConfigs'));
        }

        return ({
            GetFileGeneratorTemplateConfigs: GetFileGeneratorTemplateConfigs,
        });
    }

    appControllers.service('VRAnalytic_SendEmailHandlerAPIService', sendEmailHandlerAPIService);

})(appControllers);