﻿(function (appControllers) {

    'use strict';

    AccountBEDefinitionAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Retail_BE_ModuleConfig', 'SecurityService'];

    function AccountBEDefinitionAPIService(BaseAPIService, UtilsService, Retail_BE_ModuleConfig, SecurityService) {

        var controllerName = 'AccountBEDefinition';

        function GetAccountViewDefinitionSettingsConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, "GetAccountViewDefinitionSettingsConfigs"));
        }

        function GetAccountActionDefinitionSettingsConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, "GetAccountActionDefinitionSettingsConfigs"));
        }
        
        function GetAccountGridColumnAttributes(accountBEDefinitionId, parentAccountId) {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, "GetAccountGridColumnAttributes"), {
                accountBEDefinitionId: accountBEDefinitionId,
                parentAccountId: parentAccountId
            });
        }

        function GetAccountViewDefinitions(accountBEDefinitionId) {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, "GetAccountViewDefinitions"), {
                accountBEDefinitionId: accountBEDefinitionId
            });
        }

        function GetAccountViewDefinitionsByAccountId(accountBEDefinitionId, accountId) {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, "GetAccountViewDefinitionsByAccountId"),
                { accountId: accountId });
        }
        

        return {
            GetAccountViewDefinitionSettingsConfigs: GetAccountViewDefinitionSettingsConfigs,
            GetAccountActionDefinitionSettingsConfigs: GetAccountActionDefinitionSettingsConfigs,
            GetAccountGridColumnAttributes: GetAccountGridColumnAttributes,
            GetAccountViewDefinitions: GetAccountViewDefinitions,
            GetAccountViewDefinitionsByAccountId: GetAccountViewDefinitionsByAccountId
        };
    }

    appControllers.service('Retail_BE_AccountBEDefinitionAPIService', AccountBEDefinitionAPIService);

})(appControllers);