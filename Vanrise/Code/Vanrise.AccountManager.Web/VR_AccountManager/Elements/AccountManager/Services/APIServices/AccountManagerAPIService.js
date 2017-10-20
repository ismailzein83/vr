(function (appControllers) {
    "use strict";
    accountManagerAPIService.$inject = ["BaseAPIService", "UtilsService", "VR_AccountManager_ModuleConfig", "SecurityService"];
    function accountManagerAPIService(BaseAPIService, UtilsService, VR_AccountManager_ModuleConfig, SecurityService) {
        var controllerName = "AccountManager";
        function GetFilteredAccountManagers(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_AccountManager_ModuleConfig.moduleName, controllerName, "GetFilteredAccountManagers"), input);
        }
        return ({
            GetFilteredAccountManagers: GetFilteredAccountManagers,
        });
    }
    appControllers.service("VR_AccountManager_AccountManagerAPIService", accountManagerAPIService);
})(appControllers);