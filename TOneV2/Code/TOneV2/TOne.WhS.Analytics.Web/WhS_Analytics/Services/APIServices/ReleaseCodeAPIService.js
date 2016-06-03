(function (appControllers) {

    "use strict";
    releaseCodeAPIService.$inject = ["BaseAPIService", "UtilsService", "WhS_Analytics_ModuleConfig"];

    function releaseCodeAPIService(BaseAPIService, UtilsService, WhS_Analytics_ModuleConfig) {

        var controllerName = "BlockedAttempts";

        function GetAllFilteredReleaseCodes(input) {

            return BaseAPIService.post(UtilsService.getServiceURL(WhS_Analytics_ModuleConfig.moduleName, "ReleaseCode", "GetAllFilteredReleaseCodes"), input);
        }

        return ({
            GetAllFilteredReleaseCodes: GetAllFilteredReleaseCodes
        });
    }

    appControllers.service("WhS_Analytics_ReleaseCodeAPIService", releaseCodeAPIService);
})(appControllers);