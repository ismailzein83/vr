﻿(function (appControllers) {

    'use strict';

    FinancialAccountAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Retail_BE_ModuleConfig', 'SecurityService'];

    function FinancialAccountAPIService(BaseAPIService, UtilsService, Retail_BE_ModuleConfig, SecurityService) {
        var controllerName = 'FinancialAccount';

        function GetFinancialAccountEditorRuntime(accountBEDefinitionId,accountId, sequenceNumber) {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, "GetFinancialAccountEditorRuntime"), {
                accountBEDefinitionId: accountBEDefinitionId,
                accountId: accountId,
                sequenceNumber: sequenceNumber
            });
        }
        function CheckAllowAddFinancialAccounts(accountBEDefinitionId, accountId) {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, "CheckAllowAddFinancialAccounts"), {
                accountBEDefinitionId: accountBEDefinitionId,
                accountId: accountId,
            });
        }
        function GetFilteredFinancialAccounts(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, "GetFilteredFinancialAccounts"), input);
        }

        function AddFinancialAccount(financialAccountToInsert) {
            return BaseAPIService.post(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, "AddFinancialAccount"), financialAccountToInsert);
        }

        function UpdateFinancialAccount(financialAccountToEdit) {
            return BaseAPIService.post(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, "UpdateFinancialAccount"), financialAccountToEdit);
        }

        function GetFinancialAccountsInfo(accountBEDefinitionId, filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, "GetFinancialAccountsInfo"), {
                accountBEDefinitionId:accountBEDefinitionId,
                filter: filter,
            });
        }

        function GetAccountIdsByFinancialAccountIds(accountBEDefinitionId, financialAccountIds) {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, "GetAccountIdsByFinancialAccountIds"), {
                accountBEDefinitionId: accountBEDefinitionId,
                financialAccountIds: financialAccountIds,
            });
        }

        function GetAccountCurrency(accountBEDefinitionId, financialAccountId) {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, "GetAccountCurrency"), {
                accountBEDefinitionId: accountBEDefinitionId,
                financialAccountId: financialAccountId
            });
        }

        return {
            GetFilteredFinancialAccounts: GetFilteredFinancialAccounts,
            AddFinancialAccount: AddFinancialAccount,
            UpdateFinancialAccount: UpdateFinancialAccount,
            GetFinancialAccountEditorRuntime: GetFinancialAccountEditorRuntime,
            CheckAllowAddFinancialAccounts: CheckAllowAddFinancialAccounts,
            GetFinancialAccountsInfo: GetFinancialAccountsInfo,
            GetAccountIdsByFinancialAccountIds: GetAccountIdsByFinancialAccountIds,
            GetAccountCurrency: GetAccountCurrency
        };
    }

    appControllers.service('Retail_BE_FinancialAccountAPIService', FinancialAccountAPIService);

})(appControllers);