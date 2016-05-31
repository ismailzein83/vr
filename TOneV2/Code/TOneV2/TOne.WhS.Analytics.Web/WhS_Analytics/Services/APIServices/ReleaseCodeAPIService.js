(function (appControllers) {

    "use strict";
    releaseCodeAPIService.$inject = ["BaseAPIService", "UtilsService", "WhS_Analytics_ModuleConfig"];

    function releaseCodeAPIService(BaseAPIService, UtilsService, WhS_Analytics_ModuleConfig) {

        var controllerName = "BlockedAttempts";

        function GetReleaseCodeData(input) {

            return BaseAPIService.post(UtilsService.getServiceURL(WhS_Analytics_ModuleConfig.moduleName, "ReleaseCode", "GetReleaseCodeData"), input);
        }

        return ({
            GetReleaseCodeData: GetReleaseCodeData
        });
    }

    appControllers.service("WhS_Analytics_ReleaseCodeAPIService", releaseCodeAPIService);
})(appControllers);