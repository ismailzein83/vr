(function (appControllers) {

    'use strict';

    AccountBEAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Retail_BE_ModuleConfig', 'SecurityService'];

    function AccountBEAPIService(BaseAPIService, UtilsService, Retail_BE_ModuleConfig, SecurityService) {

        var controllerName = 'AccountBE';

        function GetFilteredAccounts(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, 'GetFilteredAccounts'), input);
        }

        function GetAccount(accountBEDefinitionId, accountId) {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, 'GetAccount'), {
                accountBEDefinitionId: accountBEDefinitionId,
                accountId: accountId
            });
        }

        function GetAccountName(accountBEDefinitionId, accountId) {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, 'GetAccountName'), {
                accountBEDefinitionId: accountBEDefinitionId,
                accountId: accountId
            });
        }

        function GetAccountDetail(accountBEDefinitionId, accountId) {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, "GetAccountDetail"), {
                accountBEDefinitionId: accountBEDefinitionId,
                accountId: accountId
            });
        }

        function AddAccount(accountToInsert) {
            return BaseAPIService.post(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, 'AddAccount'), accountToInsert);
        }

        function UpdateAccount(accountToEdit) {
            return BaseAPIService.post(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, 'UpdateAccount'), accountToEdit);
        }

        function GetAccountsInfo(accountBEDefinitionId, nameFilter, serializedFilter) {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, 'GetAccountsInfo'), {
                accountBEDefinitionId: accountBEDefinitionId,
                nameFilter: nameFilter,
                serializedFilter: serializedFilter
            });
        }

        function GetAccountsInfoByIds(filter) {
            return BaseAPIService.post(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, 'GetAccountsInfoByIds'), filter);
        }


        function GetAccountEditorRuntime(accountBEDefinitionId, accountTypeId, parentAccountId) {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, 'GetAccountEditorRuntime'), {
                accountBEDefinitionId: accountBEDefinitionId,
                accountTypeId: accountTypeId,
                parentAccountId: parentAccountId
            });
        }

        function DoesUserHaveAddAccess(accountBEDefinitionId) {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, "DoesUserHaveAddAccess"), {
                accountBEDefinitionId: accountBEDefinitionId
            }, {
                useCache: true
            });
        }
        //function HasViewAccountsPermission() {
        //    return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(Retail_BE_ModuleConfig.moduleName, controllerName, ['GetFilteredAccounts']));
        //}

        //function HasUpdateAccountPermission() {
        //    return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(Retail_BE_ModuleConfig.moduleName, controllerName, ['UpdateAccount']));
        //}

        //function GetAccountDetail(accountId) {
        //    return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, "GetAccountDetail"), {
        //        accountId: accountId
        //    });
        //}

        function ExecuteAccountBulkActions(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, "ExecuteAccountBulkActions"), input);
        }

        return {
            GetFilteredAccounts: GetFilteredAccounts,
            GetAccount: GetAccount,
            GetAccountName: GetAccountName,
            GetAccountDetail: GetAccountDetail,
            AddAccount: AddAccount,
            UpdateAccount: UpdateAccount,
            GetAccountsInfo: GetAccountsInfo,
            GetAccountsInfoByIds: GetAccountsInfoByIds,
            GetAccountEditorRuntime: GetAccountEditorRuntime,
            DoesUserHaveAddAccess: DoesUserHaveAddAccess,
            //HasViewAccountsPermission: HasViewAccountsPermission,
            //HasAddAccountPermission: HasAddAccountPermission,
            //HasUpdateAccountPermission: HasUpdateAccountPermission,
            ExecuteAccountBulkActions: ExecuteAccountBulkActions
        };
    }

    appControllers.service('Retail_BE_AccountBEAPIService', AccountBEAPIService);

})(appControllers);