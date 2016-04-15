(function (appControllers) {

    "use strict";
    AnalyticConfigurationAPIService.$inject = ['BaseAPIService', 'VR_Analytic_ModuleConfig', 'UtilsService', 'SecurityService'];

    function AnalyticConfigurationAPIService(BaseAPIService, VR_Analytic_ModuleConfig, UtilsService, SecurityService) {
        var controllerName = 'AnalyticConfiguration';

        function GetDimensionsInfo() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Analytic_ModuleConfig.moduleName, controllerName, "GetDimensionsInfo"));
        }
        function GetMeasuresInfo() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Analytic_ModuleConfig.moduleName, controllerName, "GetMeasuresInfo"));
        }
        function GetAnalyticRecords(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Analytic_ModuleConfig.moduleName, controllerName, "GetAnalyticRecords"), input);
        }

        function GetFilteredRecords(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Analytic_ModuleConfig.moduleName, controllerName, "GetFilteredRecords"), input);
        }
        return ({
            GetDimensionsInfo: GetDimensionsInfo,
            GetMeasuresInfo: GetMeasuresInfo,
            GetFilteredRecords: GetFilteredRecords,
            GetAnalyticRecords: GetAnalyticRecords
        });
    }

    appControllers.service('VR_Analytic_AnalyticConfigurationAPIService', AnalyticConfigurationAPIService);

})(appControllers);