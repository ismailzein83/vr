(function (appControllers) {

    'use strict';

    AccountAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Retail_BE_ModuleConfig', 'SecurityService'];

    function AccountAPIService(BaseAPIService, UtilsService, Retail_BE_ModuleConfig, SecurityService)
    {
        var controllerName = 'Account';

        function GetFilteredAccounts(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, 'GetFilteredAccounts'), input);
        }

        function GetAccount(accountId) {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, 'GetAccount'), {
                accountId: accountId
            });
        }

        function GetAccountName(accountId) {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, 'GetAccountName'), {
                accountId: accountId
            });
        }

        function AddAccount(account) {
            return BaseAPIService.post(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, 'AddAccount'), account);
        }

        function UpdateAccount(account) {
            return BaseAPIService.post(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, 'UpdateAccount'), account);
        }

        function HasAddAccountPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(Retail_BE_ModuleConfig.moduleName, controllerName, ['AddAccount']));
        }

        function HasUpdateAccountPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(Retail_BE_ModuleConfig.moduleName, controllerName, ['UpdateAccount']));
        }

        return {
            GetFilteredAccounts: GetFilteredAccounts,
            GetAccount: GetAccount,
            GetAccountName: GetAccountName,
            AddAccount: AddAccount,
            UpdateAccount: UpdateAccount,
            HasAddAccountPermission: HasAddAccountPermission,
            HasUpdateAccountPermission: HasUpdateAccountPermission
        };
    }

    appControllers.service('Retail_BE_AccountAPIService', AccountAPIService);

})(appControllers);