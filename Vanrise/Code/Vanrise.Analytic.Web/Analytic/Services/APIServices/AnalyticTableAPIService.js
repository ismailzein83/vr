(function (appControllers) {

    "use strict";
    AnalyticTableAPIService.$inject = ['BaseAPIService', 'VR_Analytic_ModuleConfig', 'UtilsService', 'SecurityService'];

    function AnalyticTableAPIService(BaseAPIService, VR_Analytic_ModuleConfig, UtilsService, SecurityService) {
        var controllerName = 'AnalyticTable';

        function GetAnalyticTablesInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Analytic_ModuleConfig.moduleName, controllerName, "GetAnalyticTablesInfo"),
                {
                    filter: filter
                });
        }
        return ({
            GetAnalyticTablesInfo: GetAnalyticTablesInfo
        });
    }

    appControllers.service('VR_Analytic_AnalyticTableAPIService', AnalyticTableAPIService);

})(appControllers);