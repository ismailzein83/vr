(function (appControllers) {

    'use strict';

    BillingTransactionAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_AccountBalance_ModuleConfig', 'SecurityService'];

    function BillingTransactionAPIService(BaseAPIService, UtilsService, VR_AccountBalance_ModuleConfig, SecurityService) {
        var controllerName = 'BillingTransaction';

        function GetFilteredBillingTransactions(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_AccountBalance_ModuleConfig.moduleName, controllerName, "GetFilteredBillingTransactions"), input);
        }


        function AddBillingTransaction(billingTransaction) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_AccountBalance_ModuleConfig.moduleName, controllerName, 'AddBillingTransaction'), billingTransaction);
        }

        function HasAddBillingTransactionPermission(accountTypeId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_AccountBalance_ModuleConfig.moduleName, controllerName, 'DoesUserHaveAddAccess'),
                {
                    accountTypeId : accountTypeId
                });
        }

        return {
            GetFilteredBillingTransactions: GetFilteredBillingTransactions,
            AddBillingTransaction: AddBillingTransaction,
            HasAddBillingTransactionPermission: HasAddBillingTransactionPermission
        };
    }

    appControllers.service('VR_AccountBalance_BillingTransactionAPIService', BillingTransactionAPIService);

})(appControllers);