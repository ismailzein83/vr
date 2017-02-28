﻿(function (appControllers) {

    'use strict';

    FinancialAccountAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_AccountBalance_ModuleConfig', 'SecurityService'];

    function FinancialAccountAPIService(BaseAPIService, UtilsService, WhS_AccountBalance_ModuleConfig, SecurityService) {

        var controllerName = 'FinancialAccount';
        function GetFilteredFinancialAccounts(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_AccountBalance_ModuleConfig.moduleName, controllerName, "GetFilteredFinancialAccounts"), input);
        }

        function AddFinancialAccount(financialAccountObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_AccountBalance_ModuleConfig.moduleName, controllerName, "AddFinancialAccount"), financialAccountObject);
        }
        function UpdateFinancialAccount(financialAccountObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_AccountBalance_ModuleConfig.moduleName, controllerName, "UpdateFinancialAccount"), financialAccountObject);
        }
        return {
            GetFilteredFinancialAccounts: GetFilteredFinancialAccounts,
            AddFinancialAccount: AddFinancialAccount,
            UpdateFinancialAccount: UpdateFinancialAccount
        };
    }

    appControllers.service('WhS_AccountBalance_FinancialAccountAPIService', FinancialAccountAPIService);

})(appControllers);