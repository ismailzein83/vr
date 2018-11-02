(function (appControllers) {
    "use strict";

    BlockedAttemptsAPIService.$inject = ['BaseAPIService', 'UtilsService', 'SecurityService', 'CP_WhS_ModuleConfig'];

    function BlockedAttemptsAPIService(BaseAPIService, UtilsService, SecurityService, CP_WhS_ModuleConfig) {
        var controllerName = "BlockedAttempts";
        function GetFilteredBlockedAttempts(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(CP_WhS_ModuleConfig.moduleName, controllerName, 'GetFilteredBlockedAttempts'), input);
        }
        return ({
            GetFilteredBlockedAttempts: GetFilteredBlockedAttempts
        });
    }
    appControllers.service('CP_WhS_BlockedAttemptsAPIService', BlockedAttemptsAPIService);

})(appControllers);