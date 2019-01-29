(function (appControllers) {

    "use strict";
    whSJazzRegionAPIService.$inject = ["BaseAPIService", "UtilsService", "WhS_Jazz_ModuleConfig"];

    function whSJazzRegionAPIService(BaseAPIService, UtilsService, WhS_Jazz_ModuleConfig) {

        var controllerName = "Region";

        function GetAllRegions() {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Jazz_ModuleConfig.moduleName, controllerName, 'GetAllRegions'), {
            });
        }
        function GetRegionsInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Jazz_ModuleConfig.moduleName, controllerName, 'GetRegionsInfo'), {
                filter:filter
            });
        }
        return ({
            GetAllRegions: GetAllRegions,
            GetRegionsInfo: GetRegionsInfo
        });
    }

    appControllers.service("WhS_Jazz_RegionAPIService", whSJazzRegionAPIService);
})(appControllers);