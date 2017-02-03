(function (appControllers) {

    'use strict';

    Retail_BE_AccountTypeAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Retail_BE_ModuleConfig', 'SecurityService'];

    function Retail_BE_AccountTypeAPIService(BaseAPIService, UtilsService, Retail_BE_ModuleConfig, SecurityService) {
        var controllerName = 'AccountType';

        function GetFilteredAccountTypes(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, 'GetFilteredAccountTypes'), input);
        }

        function GetAccountType(accountTypeId) {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, 'GetAccountType'), {
                accountTypeId: accountTypeId
            });
        }

        function GetAccountTypesInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, "GetAccountTypesInfo"), {
                filter: filter
            });
        }

        function GetAccountTypePartDefinitionExtensionConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, "GetAccountTypePartDefinitionExtensionConfigs"));
        }

        function AddAccountType(accountType) {
            return BaseAPIService.post(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, 'AddAccountType'), accountType);
        }

        function UpdateAccountType(accountType) {
            return BaseAPIService.post(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, 'UpdateAccountType'), accountType);
        }

        function GetGenericFieldDefinitionsInfo(accountBEDefinitionId) {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, 'GetGenericFieldDefinitionsInfo'), { accountBEDefinitionId: accountBEDefinitionId }, { useCache: true });
        }

        function GetAccountGenericFieldValues(accountBEDefinitionId, accountId, serializedAccountGenericFieldNames) {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, 'GetAccountGenericFieldValues'), {
                accountBEDefinitionId: accountBEDefinitionId,
                accountId: accountId,
                serializedAccountGenericFieldNames: serializedAccountGenericFieldNames
            });
        }

        function HasViewAccountTypesPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(Retail_BE_ModuleConfig.moduleName, controllerName, ['GetFilteredAccountTypes']));
        }

        function HasAddAccountTypePermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(Retail_BE_ModuleConfig.moduleName, controllerName, ['AddAccountType']));
        }

        function HasUpdateAccountTypePermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(Retail_BE_ModuleConfig.moduleName, controllerName, ['UpdateAccountType']));
        }

        return {
            GetFilteredAccountTypes: GetFilteredAccountTypes,
            GetAccountType: GetAccountType,
            GetAccountTypesInfo: GetAccountTypesInfo,
            GetAccountTypePartDefinitionExtensionConfigs: GetAccountTypePartDefinitionExtensionConfigs,
            AddAccountType: AddAccountType,
            UpdateAccountType: UpdateAccountType,
            GetGenericFieldDefinitionsInfo: GetGenericFieldDefinitionsInfo,
            GetAccountGenericFieldValues: GetAccountGenericFieldValues,
            HasViewAccountTypesPermission: HasViewAccountTypesPermission,
            HasAddAccountTypePermission: HasAddAccountTypePermission,
            HasUpdateAccountTypePermission: HasUpdateAccountTypePermission
        };
    }

    appControllers.service('Retail_BE_AccountTypeAPIService', Retail_BE_AccountTypeAPIService);

})(appControllers);