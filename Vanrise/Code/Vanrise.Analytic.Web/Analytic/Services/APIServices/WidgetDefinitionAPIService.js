(function (appControllers) {

    "use strict";
    WidgetDefinitionAPIService.$inject = ['BaseAPIService', 'VR_Analytic_ModuleConfig', 'UtilsService', 'SecurityService'];

    function WidgetDefinitionAPIService(BaseAPIService, VR_Analytic_ModuleConfig, UtilsService, SecurityService) {
        var controllerName = 'AnalyticTable';

        function GetWidgetDefinitions() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Analytic_ModuleConfig.moduleName, controllerName, "GetWidgetDefinitions"));
        }
        return ({
            GetWidgetDefinitions: GetWidgetDefinitions
        });
    }

    appControllers.service('VR_Analytic_WidgetDefinitionAPIService', WidgetDefinitionAPIService);

})(appControllers);