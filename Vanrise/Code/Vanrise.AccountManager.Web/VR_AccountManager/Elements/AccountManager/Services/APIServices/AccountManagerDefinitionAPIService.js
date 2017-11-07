(function (appControllers) {
    "use strict";
    accountManagerDefinitionAPIService.$inject = ["BaseAPIService", "UtilsService", "VR_AccountManager_ModuleConfig", "SecurityService"];
    function accountManagerDefinitionAPIService(BaseAPIService, UtilsService, VR_AccountManager_ModuleConfig, SecurityService) {
        var controllerName = "AccountManagerDefinition";
        function GetAccountManagerDefinition(accountManagerDefinitionId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_AccountManager_ModuleConfig.moduleName, controllerName, "GetAccountManagerDefinition"), { accountManagerDefinitionId: accountManagerDefinitionId });
        }
        function GetAssignmentDefinitionConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_AccountManager_ModuleConfig.moduleName, controllerName, "GetAssignmentDefinitionConfigs"));
        }
        function GetSubViewsDefinitionConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_AccountManager_ModuleConfig.moduleName, controllerName, "GetSubViewsDefinitionConfigs"));
        }
        function GetAccountManagerSubViewsDefinition(accountManagerDefinitionId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_AccountManager_ModuleConfig.moduleName, controllerName, "GetAccountManagerSubViewsDefinition"), { accountManagerDefinitionId: accountManagerDefinitionId });
        }
        function GetAccountManagerAssignmentRuntimeEditor(accountManagerRuntimeInput) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_AccountManager_ModuleConfig.moduleName, controllerName, "GetAccountManagerAssignmentRuntimeEditor"),  accountManagerRuntimeInput);
        }
        return ({
            GetAccountManagerDefinition: GetAccountManagerDefinition,
            GetAssignmentDefinitionConfigs: GetAssignmentDefinitionConfigs,
            GetSubViewsDefinitionConfigs: GetSubViewsDefinitionConfigs,
            GetAccountManagerSubViewsDefinition: GetAccountManagerSubViewsDefinition,
            GetAccountManagerAssignmentRuntimeEditor: GetAccountManagerAssignmentRuntimeEditor

        });
    }
    appControllers.service("VR_AccountManager_AccountManagerDefinitionAPIService", accountManagerDefinitionAPIService);
})(appControllers);