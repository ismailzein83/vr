(function (appControllers) {

    'use strict';

    AccountManagerAssignmentAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Retail_BE_ModuleConfig', 'SecurityService'];

    function AccountManagerAssignmentAPIService(BaseAPIService, UtilsService, Retail_BE_ModuleConfig, SecurityService) {
        var controllerName = 'AccountManagerAssignment';

        function GetAccountManagerAssignments(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, "GetAccountManagerAssignments"),input);
        }
        function AddAccountManagerAssignment(accountmanagerAssignment) {
            return BaseAPIService.post(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, "AddAccountManagerAssignment"), accountmanagerAssignment);
        }
        function UpdateAccountManagerAssignment(accountmanagerAssignment) {
            return BaseAPIService.post(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, "UpdateAccountManagerAssignment"), accountmanagerAssignment);
        }
        function GetAccountManagerAssignmentRuntimeEditor(accountManagerRuntimeInput) {
            return BaseAPIService.post(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, "GetAccountManagerAssignmentRuntimeEditor"), accountManagerRuntimeInput);
        }
        function GetAccountManagerDefInfo(accountBeDefinitionId) {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, "GetAccountManagerDefInfo"), { accountBeDefinitionId: accountBeDefinitionId });
        }
        function IsAccountAssignedToAccountManager(accountId, accountBeDefinitionId) {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, "IsAccountAssignedToAccountManager"), { accountId: accountId, accountBeDefinitionId: accountBeDefinitionId });
        }
        return {
            GetAccountManagerAssignments: GetAccountManagerAssignments,
            AddAccountManagerAssignment: AddAccountManagerAssignment,
            UpdateAccountManagerAssignment: UpdateAccountManagerAssignment,
            GetAccountManagerAssignmentRuntimeEditor: GetAccountManagerAssignmentRuntimeEditor,
            GetAccountManagerDefInfo: GetAccountManagerDefInfo,
            IsAccountAssignedToAccountManager: IsAccountAssignedToAccountManager
        };
    }

    appControllers.service('Retail_BE_AccountManagerAssignmentAPIService', AccountManagerAssignmentAPIService);

})(appControllers);