(function (appControllers) {

    'use strict';

    AccountTypeAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_AccountBalance_ModuleConfig', 'SecurityService'];

    function AccountTypeAPIService(BaseAPIService, UtilsService, VR_AccountBalance_ModuleConfig, SecurityService) {
        var controllerName = 'AccountType';
        function GetAccountSelector(accountTypeId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_AccountBalance_ModuleConfig.moduleName, controllerName, "GetAccountSelector"), { accountTypeId: accountTypeId });
        }
        function GetAccountTypeSettings(accountTypeId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_AccountBalance_ModuleConfig.moduleName, controllerName, "GetAccountTypeSettings"), { accountTypeId: accountTypeId });
        }
        return {
            GetAccountSelector: GetAccountSelector,
            GetAccountTypeSettings: GetAccountTypeSettings
        };
    }

    appControllers.service('VR_AccountBalance_AccountTypeAPIService', AccountTypeAPIService);

})(appControllers);