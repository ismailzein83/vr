﻿(function (appControllers) {

    'use strict';

    AccountBEDefinitionAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Retail_BE_ModuleConfig', 'SecurityService'];

    function AccountBEDefinitionAPIService(BaseAPIService, UtilsService, Retail_BE_ModuleConfig, SecurityService) {

        var controllerName = 'AccountBEDefinition';

        function GetAccountBEDefinitionSettingsWithHidden(accountBEDefinitionId) {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, "GetAccountBEDefinitionSettingsWithHidden"), {
                accountBEDefinitionId: accountBEDefinitionId
            });
        }

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
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, "GetAccountViewDefinitionsByAccountId"), {
                accountBEDefinitionId: accountBEDefinitionId,
                accountId: accountId
            });
        }
        
        function GetAccountActionDefinitions(accountBEDefinitionId) {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, "GetAccountActionDefinitions"), {
                accountBEDefinitionId: accountBEDefinitionId
            });
        }

        function GetAccountActionGroupDefinitions(accountBEDefinitionId) {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, "GetAccountActionGroupDefinitions"), {
                accountBEDefinitionId: accountBEDefinitionId
            });
        }

        function GetAccountActionDefinitionsInfo(accountBEDefinitionId, serializedFilter) {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, "GetAccountActionDefinitionsInfo"), {
                accountBEDefinitionId: accountBEDefinitionId,
                serializedFilter: serializedFilter
            });
        }
        function GetAccountActionDefinition(accountBEDefinitionId, actionDefinitionId) {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, "GetAccountActionDefinition"), {
                accountBEDefinitionId: accountBEDefinitionId,
                actionDefinitionId: actionDefinitionId
            });
        }

        function GetAccountExtraFieldDefinitionSettingsConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, "GetAccountExtraFieldDefinitionSettingsConfigs"));
        }
        function CheckUseRemoteSelector(accountBEDefinitionId) {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, "CheckUseRemoteSelector"), {
                accountBEDefinitionId: accountBEDefinitionId
            });
        }
        function GetFinancialAccountLocatorConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, "GetFinancialAccountLocatorConfigs"));
        }
        function GetAccountBEStatusDefinitionId(accountBEDefinitionId) {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, "GetAccountBEStatusDefinitionId"), {
                accountBEDefinitionId: accountBEDefinitionId,
            });
        }
        function GetAccountBulkActionSettingsConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, "GetAccountBulkActionSettingsConfigs"));
        }
        function GetAccountBEDefinitionClassificationsInfo(accountBEDefinitionId) {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, "GetAccountBEDefinitionClassificationsInfo"), {
                accountBEDefinitionId: accountBEDefinitionId
            });
        }

        function CheckUseRecurringChargeModule(accountBEDefinitionId) {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, "CheckUseRecurringChargeModule"), {
                accountBEDefinitionId: accountBEDefinitionId
            });
        }

        return {
            GetAccountBEDefinitionSettingsWithHidden: GetAccountBEDefinitionSettingsWithHidden,
            GetAccountViewDefinitionSettingsConfigs: GetAccountViewDefinitionSettingsConfigs,
            GetAccountActionDefinitionSettingsConfigs: GetAccountActionDefinitionSettingsConfigs,
            GetAccountGridColumnAttributes: GetAccountGridColumnAttributes,
            GetAccountViewDefinitions: GetAccountViewDefinitions,
            GetAccountViewDefinitionsByAccountId: GetAccountViewDefinitionsByAccountId,
            GetAccountActionDefinitions: GetAccountActionDefinitions,
            GetAccountActionGroupDefinitions: GetAccountActionGroupDefinitions,
            GetAccountActionDefinitionsInfo: GetAccountActionDefinitionsInfo,
            GetAccountActionDefinition: GetAccountActionDefinition,
            GetAccountExtraFieldDefinitionSettingsConfigs: GetAccountExtraFieldDefinitionSettingsConfigs,
            CheckUseRemoteSelector: CheckUseRemoteSelector,
            GetFinancialAccountLocatorConfigs: GetFinancialAccountLocatorConfigs,
            GetAccountBEStatusDefinitionId: GetAccountBEStatusDefinitionId,
            GetAccountBulkActionSettingsConfigs: GetAccountBulkActionSettingsConfigs,
            GetAccountBEDefinitionClassificationsInfo: GetAccountBEDefinitionClassificationsInfo,
            CheckUseRecurringChargeModule: CheckUseRecurringChargeModule
        };
    }

    appControllers.service('Retail_BE_AccountBEDefinitionAPIService', AccountBEDefinitionAPIService);

})(appControllers);