(function (appControllers) {

    "use strict";
    whSJazzRegionCodeAPIService.$inject = ["BaseAPIService", "UtilsService", "WhS_Jazz_ModuleConfig"];

    function whSJazzRegionCodeAPIService(BaseAPIService, UtilsService, WhS_Jazz_ModuleConfig) {

        var controllerName = "WhSJazzRegionCode";

        function GetAllRegionCodes() {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Jazz_ModuleConfig.moduleName, controllerName, 'GetAllRegionCodes'), {
            });
        }
        function GetRegionCodesInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Jazz_ModuleConfig.moduleName, controllerName, 'GetRegionCodesInfo'), {
                filter:filter
            });
        }
        return ({
            GetAllRegionCodes: GetAllRegionCodes,
            GetRegionCodesInfo: GetRegionCodesInfo
        });
    }

    appControllers.service("WhS_Jazz_RegionCodeAPIService", whSJazzRegionCodeAPIService);
})(appControllers);