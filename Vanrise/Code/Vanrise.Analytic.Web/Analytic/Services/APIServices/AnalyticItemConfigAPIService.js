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
        
        return ({
            GetDimensionsInfo: GetDimensionsInfo,
            GetMeasuresInfo: GetMeasuresInfo,
        });
    }

    appControllers.service('VR_Analytic_AnalyticItemConfigAPIService', AnalyticItemConfigAPIService);

})(appControllers);