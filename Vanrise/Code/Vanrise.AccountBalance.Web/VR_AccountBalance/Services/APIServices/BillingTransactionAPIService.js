(function (appControllers) {

    'use strict';

    BillingTransactionAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_AccountBalance_ModuleConfig', 'SecurityService'];

    function BillingTransactionAPIService(BaseAPIService, UtilsService, VR_AccountBalance_ModuleConfig, SecurityService) {
        var controllerName = 'BillingTransaction';

        function GetFilteredBillingTransactions(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_AccountBalance_ModuleConfig.moduleName, controllerName, "GetFilteredBillingTransactions"), input);
        }

        return {
            GetFilteredBillingTransactions: GetFilteredBillingTransactions
        };
    }

    appControllers.service('VR_AccountBalance_BillingTransactionAPIService', BillingTransactionAPIService);

})(appControllers);