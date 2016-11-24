
(function (appControllers) {

    "use strict";
    AccountAPIService.$inject = ['BaseAPIService', 'UtilsService', 'NP_IVSwitch_ModuleConfig', 'SecurityService'];

    function AccountAPIService(BaseAPIService, UtilsService, NP_IVSwitch_ModuleConfig, SecurityService) {

        var controllerName = "Account";


        function GetFilteredAccounts(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(NP_IVSwitch_ModuleConfig.moduleName, controllerName, 'GetFilteredAccounts'), input);
        }

        function GetAccount(AccountId) {
            return BaseAPIService.get(UtilsService.getServiceURL(NP_IVSwitch_ModuleConfig.moduleName, controllerName, 'GetAccount'), {
                AccountId: AccountId
            });
        }

        function AddAccount(AccountItem) {
            console.log(AccountItem)
            return BaseAPIService.post(UtilsService.getServiceURL(NP_IVSwitch_ModuleConfig.moduleName, controllerName, 'AddAccount'), AccountItem);
        }

        function UpdateAccount(AccountItem) {
            return BaseAPIService.post(UtilsService.getServiceURL(NP_IVSwitch_ModuleConfig.moduleName, controllerName, 'UpdateAccount'), AccountItem);
        }
 

        function HasAddAccountPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(NP_IVSwitch_ModuleConfig.moduleName, controllerName, ['AddAccount']));
        }

        function HasEditAccountPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(NP_IVSwitch_ModuleConfig.moduleName, controllerName, ['UpdateAccount']));
        }


        return ({
            GetFilteredAccounts: GetFilteredAccounts,
            GetAccount: GetAccount,
            AddAccount: AddAccount,
            UpdateAccount: UpdateAccount,
            HasAddAccountPermission: HasAddAccountPermission,
            HasEditAccountPermission: HasEditAccountPermission,
        });
    }

    appControllers.service('NP_IVSwitch_AccountAPIService', AccountAPIService);

})(appControllers);