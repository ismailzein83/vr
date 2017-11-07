(function (appControllers) {

    'use strict';

    AccountManagerAssignmentAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Retail_BE_ModuleConfig', 'SecurityService'];

    function AccountManagerAssignmentAPIService(BaseAPIService, UtilsService, Retail_BE_ModuleConfig, SecurityService) {
        var controllerName = 'AccountManagerAssignment';

        function GetAccountManagerAssignments() {
            return BaseAPIService.post(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, "GetAccountManagerAssignments"));
        }
        function AddAccountManagerAssignment(accountmanagerAssignment) {
            return BaseAPIService.post(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, "AddAccountManagerAssignment"), accountmanagerAssignment);
        }
        function UpdateAccountManagerAssignment(accountmanagerAssignment) {
            return BaseAPIService.post(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, "UpdateAccountManagerAssignment"), accountmanagerAssignment);
        }
        return {
            GetAccountManagerAssignments: GetAccountManagerAssignments,
            AddAccountManagerAssignment: AddAccountManagerAssignment,
            UpdateAccountManagerAssignment: UpdateAccountManagerAssignment
        };
    }

    appControllers.service('Retail_BE_AccountManagerAssignmentAPIService', AccountManagerAssignmentAPIService);

})(appControllers);