(function (appControllers) {

    "use strict";
    blockedAttemptsAPIService.$inject = ["BaseAPIService", "UtilsService", "WhS_Analytics_ModuleConfig"];

    function blockedAttemptsAPIService(BaseAPIService, UtilsService, WhS_Analytics_ModuleConfig) {

        var controllerName = "BlockedAttempts";

        function GetBlockedAttemptsData(input) {

            return BaseAPIService.post(UtilsService.getServiceURL(WhS_Analytics_ModuleConfig.moduleName, "BlockedAttempts", "GetBlockedAttemptsData"), input);
        }

        return ({
            GetBlockedAttemptsData: GetBlockedAttemptsData
        });
    }

    appControllers.service("WhS_Analytics_BlockedAttemptsAPIService", blockedAttemptsAPIService);
})(appControllers);