﻿(function (appControllers) {

    'use strict';

    BillingTransactionAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_AccountBalance_ModuleConfig', 'SecurityService'];

    function BillingTransactionAPIService(BaseAPIService, UtilsService, VR_AccountBalance_ModuleConfig, SecurityService) {
        var controllerName = 'BillingTransaction';

        function GetFilteredBillingTransactions(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_AccountBalance_ModuleConfig.moduleName, controllerName, "GetFilteredBillingTransactions"), input);
        }

        function HasViewBillingTransactionPermission(accountTypeId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_AccountBalance_ModuleConfig.moduleName, controllerName, 'DoesUserHaveViewAccess'), { accountTypeId: accountTypeId });
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

        function GetBillingTransactionById(billingTransactionId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_AccountBalance_ModuleConfig.moduleName, controllerName, 'GetBillingTransactionById'), { billingTransactionId: billingTransactionId });
        }

        return {
            GetFilteredBillingTransactions: GetFilteredBillingTransactions,
            HasViewBillingTransactionPermission: HasViewBillingTransactionPermission,
            AddBillingTransaction: AddBillingTransaction,
            HasAddBillingTransactionPermission: HasAddBillingTransactionPermission,
            GetBillingTransactionById: GetBillingTransactionById
        };
    }

    appControllers.service('VR_AccountBalance_BillingTransactionAPIService', BillingTransactionAPIService);

})(appControllers);