(function (appControllers) {

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
        function GetFinancialAccount(financialAccountId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_AccountBalance_ModuleConfig.moduleName, controllerName, "GetFinancialAccount"), {
                financialAccountId: financialAccountId
            });
        }
        function CheckCarrierAllowAddFinancialAccounts(carrierProfileId, carrierAccountId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_AccountBalance_ModuleConfig.moduleName, controllerName, "CheckCarrierAllowAddFinancialAccounts"), {
                carrierProfileId: carrierProfileId,
                carrierAccountId: carrierAccountId
            });
        }
        function GetAccountCurrencyName(carrierProfileId, carrierAccountId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_AccountBalance_ModuleConfig.moduleName, controllerName, "GetAccountCurrencyName"), {
                carrierProfileId: carrierProfileId,
                carrierAccountId: carrierAccountId
            });
        }
        return {
            GetFilteredFinancialAccounts: GetFilteredFinancialAccounts,
            AddFinancialAccount: AddFinancialAccount,
            UpdateFinancialAccount: UpdateFinancialAccount,
            CheckCarrierAllowAddFinancialAccounts: CheckCarrierAllowAddFinancialAccounts,
            GetFinancialAccount: GetFinancialAccount,
            GetAccountCurrencyName: GetAccountCurrencyName
        };
    }

    appControllers.service('WhS_AccountBalance_FinancialAccountAPIService', FinancialAccountAPIService);

})(appControllers);