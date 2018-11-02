(function (appControllers) {
    "use strict";

    ReleaseCodeStatisticsAPIService.$inject = ['BaseAPIService', 'UtilsService', 'SecurityService', 'CP_WhS_ModuleConfig'];

    function ReleaseCodeStatisticsAPIService(BaseAPIService, UtilsService, SecurityService, CP_WhS_ModuleConfig) {
        var controllerName = "ReleaseCodeStatistics";
        function GetFilteredReleaseCodeStatistics(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(CP_WhS_ModuleConfig.moduleName, controllerName, 'GetFilteredReleaseCodeStatistics'), input);
        }
        return ({
            GetFilteredReleaseCodeStatistics: GetFilteredReleaseCodeStatistics
        });
    }
    appControllers.service('CP_WhS_ReleaseCodeStatisticsAPIService', ReleaseCodeStatisticsAPIService);

})(appControllers);