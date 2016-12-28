(function (appControllers) {

    'use strict';

    AccountBEAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Retail_BE_ModuleConfig', 'SecurityService'];

    function AccountBEAPIService(BaseAPIService, UtilsService, Retail_BE_ModuleConfig, SecurityService) {

        var controllerName = 'AccountBE';

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

        //function GetAccountEditorRuntime(accountTypeId, parentAccountId) {
        //    return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, 'GetAccountEditorRuntime'), {
        //        accountTypeId: accountTypeId,
        //        parentAccountId: parentAccountId
        //    });
        //}

        //function HasAddAccountPermission() {
        //    return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(Retail_BE_ModuleConfig.moduleName, controllerName, ['AddAccount']));
        //}

        //function HasViewAccountsPermission() {
        //    return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(Retail_BE_ModuleConfig.moduleName, controllerName, ['GetFilteredAccounts']));
        //}

        //function HasUpdateAccountPermission() {
        //    return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(Retail_BE_ModuleConfig.moduleName, controllerName, ['UpdateAccount']));
        //}

        //function GetAccountsInfo(nameFilter, serializedFilter) {
        //    return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, "GetAccountsInfo"), {
        //        nameFilter: nameFilter,
        //        serializedFilter: serializedFilter
        //    });
        //}

        //function GetAccountsInfoByIds(accountIds) {
        //    return BaseAPIService.post(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, "GetAccountsInfoByIds"), accountIds);
        //}

        //function GetAccountDetail(accountId) {
        //    return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, "GetAccountDetail"), {
        //        accountId: accountId
        //    });
        //}

        return {
            GetFilteredAccounts: GetFilteredAccounts,
            GetAccountDetail: GetAccountDetail,
            GetAccount: GetAccount,
            GetAccountName: GetAccountName,
            AddAccount: AddAccount,
            UpdateAccount: UpdateAccount,
            //GetAccountEditorRuntime: GetAccountEditorRuntime,
            //HasViewAccountsPermission: HasViewAccountsPermission,
            //HasAddAccountPermission: HasAddAccountPermission,
            //HasUpdateAccountPermission: HasUpdateAccountPermission,
            //GetAccountsInfo: GetAccountsInfo,
            //GetAccountsInfoByIds: GetAccountsInfoByIds
        };
    }

    appControllers.service('Retail_BE_AccountBEAPIService', AccountBEAPIService);

})(appControllers);