(function (appControllers) {
    "use strict";
    accountManagerDefinitionAPIService.$inject = ["BaseAPIService", "UtilsService", "VR_AccountManager_ModuleConfig", "SecurityService"];
    function accountManagerDefinitionAPIService(BaseAPIService, UtilsService, VR_AccountManager_ModuleConfig, SecurityService) {
        var controllerName = "AccountManagerDefinition";
        function GetAccountManagerDefinition(accountManagerDefinitionId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_AccountManager_ModuleConfig.moduleName, controllerName, "GetAccountManagerDefinition"), { accountManagerDefinitionId: accountManagerDefinitionId });
        }
       
        return ({
            GetAccountManagerDefinition: GetAccountManagerDefinition,
        });
    }
    appControllers.service("VR_AccountManager_AccountManagerDefinitionAPIService", accountManagerDefinitionAPIService);
})(appControllers);