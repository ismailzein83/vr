(function (appControllers) {

    'use strict';

    AnalyticItemActionAPIService.$inject = ['BaseAPIService', 'VR_Analytic_ModuleConfig', 'UtilsService', 'SecurityService'];

    function AnalyticItemActionAPIService(BaseAPIService, VR_Analytic_ModuleConfig, UtilsService, SecurityService) {
        var controllerName = 'AnalyticItemAction';
        

        function GetAnalyticItemActionsTemplateConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Analytic_ModuleConfig.moduleName, controllerName, "GetAnalyticItemActionsTemplateConfigs"));
        }

        return ({
            GetAnalyticItemActionsTemplateConfigs: GetAnalyticItemActionsTemplateConfigs
        });
    };

    appControllers.service('VR_Analytic_AnalyticItemActionAPIService', AnalyticItemActionAPIService);

})(appControllers);
