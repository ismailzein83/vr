(function (appControllers) {

    "use strict";
    AnalyticAPIService.$inject = ['BaseAPIService', 'VR_Analytic_ModuleConfig', 'UtilsService', 'SecurityService'];

    function AnalyticAPIService(BaseAPIService, VR_Analytic_ModuleConfig, UtilsService, SecurityService) {
        var controllerName = 'Analytic';

        function GetFilteredRecords(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Analytic_ModuleConfig.moduleName, controllerName, "GetFilteredRecords"), input);
        }
        function GetRecordSearchFilterGroup(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Analytic_ModuleConfig.moduleName, controllerName, "GetRecordSearchFilterGroup"), input);
        }
        return ({
            GetFilteredRecords: GetFilteredRecords,
            GetRecordSearchFilterGroup: GetRecordSearchFilterGroup
        });
    }

    appControllers.service('VR_Analytic_AnalyticAPIService', AnalyticAPIService);

})(appControllers);