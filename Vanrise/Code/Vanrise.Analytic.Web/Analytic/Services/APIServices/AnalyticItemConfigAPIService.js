(function (appControllers) {

    "use strict";
    AnalyticItemConfigAPIService.$inject = ['BaseAPIService', 'VR_Analytic_ModuleConfig', 'UtilsService', 'SecurityService'];

    function AnalyticItemConfigAPIService(BaseAPIService, VR_Analytic_ModuleConfig, UtilsService, SecurityService) {
        var controllerName = 'AnalyticItemConfig';

        function GetDimensionsInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Analytic_ModuleConfig.moduleName, controllerName, "GetDimensionsInfo"), {
                filter: filter
            });
        }
        function GetMeasuresInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Analytic_ModuleConfig.moduleName, controllerName, "GetMeasuresInfo"), {
                filter: filter
            });
        }
        function GetFilteredDimensions(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Analytic_ModuleConfig.moduleName, controllerName, "GetFilteredDimensions"), input);
        }
        function GetFilteredMeasures(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Analytic_ModuleConfig.moduleName, controllerName, "GetFilteredMeasures"), input);
        }
        function GetFilteredJoins(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Analytic_ModuleConfig.moduleName, controllerName, "GetFilteredJoins"), input);
        }
        return ({
            GetDimensionsInfo: GetDimensionsInfo,
            GetMeasuresInfo: GetMeasuresInfo,
            GetFilteredDimensions: GetFilteredDimensions,
            GetFilteredMeasures: GetFilteredMeasures,
            GetFilteredJoins: GetFilteredJoins
        });
    }

    appControllers.service('VR_Analytic_AnalyticItemConfigAPIService', AnalyticItemConfigAPIService);

})(appControllers);