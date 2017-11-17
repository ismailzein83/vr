(function (appControllers) {
    "use strict";
    accountManagerAPIService.$inject = ["BaseAPIService", "UtilsService", "VR_AccountManager_ModuleConfig", "SecurityService"];
    function accountManagerAPIService(BaseAPIService, UtilsService, VR_AccountManager_ModuleConfig, SecurityService) {
        var controllerName = "AccountManager";
        function GetFilteredAccountManagers(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_AccountManager_ModuleConfig.moduleName, controllerName, "GetFilteredAccountManagers"), input);
        }
        function AddAccountManager(accountManager) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_AccountManager_ModuleConfig.moduleName, controllerName, "AddAccountManager"), accountManager);
        }
        function UpdateAccountManager(accountManager) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_AccountManager_ModuleConfig.moduleName, controllerName, "UpdateAccountManager"), accountManager);
        }
        function GetAccountManager(accountManagerId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_AccountManager_ModuleConfig.moduleName, controllerName, "GetAccountManager"), { accountManagerId: accountManagerId });
        }
        function GetAccountManagerDefinitionConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_AccountManager_ModuleConfig.moduleName, controllerName, "GetAccountManagerDefinitionConfigs"));
        }
        function GetAccountManagerInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_AccountManager_ModuleConfig.moduleName, controllerName, "GetAccountManagerInfo"), { filter: filter });
        }
        return ({
            GetFilteredAccountManagers: GetFilteredAccountManagers,
            AddAccountManager: AddAccountManager,
            UpdateAccountManager: UpdateAccountManager,
            GetAccountManager: GetAccountManager,
            GetAccountManagerDefinitionConfigs: GetAccountManagerDefinitionConfigs,
            GetAccountManagerInfo: GetAccountManagerInfo
        });
    }
    appControllers.service("VR_AccountManager_AccountManagerAPIService", accountManagerAPIService);
})(appControllers);