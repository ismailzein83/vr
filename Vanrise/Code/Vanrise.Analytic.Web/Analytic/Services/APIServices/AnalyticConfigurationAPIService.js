(function (appControllers) {

    "use strict";
    AnalyticConfigurationAPIService.$inject = ['BaseAPIService', 'VR_Analytic_ModuleConfig', 'UtilsService', 'SecurityService'];

    function AnalyticConfigurationAPIService(BaseAPIService, VR_Analytic_ModuleConfig, UtilsService, SecurityService) {
        var controllerName = 'Analytic';

        function GetDimensions() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Analytic_ModuleConfig.moduleName, controllerName, "GetDimensions"));
        }
        function GetMeasures() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Analytic_ModuleConfig.moduleName, controllerName, "GetMeasures"));
        }

        return ({
            GetDimensions: GetDimensions,
            GetMeasures: GetMeasures
        });
    }

    appControllers.service('VR_Analytic_AnalyticConfigurationAPIService', AnalyticConfigurationAPIService);

})(appControllers);