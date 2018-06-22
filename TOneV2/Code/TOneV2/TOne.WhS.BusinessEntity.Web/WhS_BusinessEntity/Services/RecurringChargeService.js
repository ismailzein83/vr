(function (appControllers) {

    'use stict';

    RecurringChargeService.$inject = ['WhS_BE_CarrierAccountService', 'WhS_BE_CarrierProfileService', 'VRUIUtilsService', 'VR_AccountBalance_BillingTransactionAPIService', 'WhS_BE_FinancialAccountAPIService'];

    function RecurringChargeService(WhS_BE_CarrierAccountService, WhS_BE_CarrierProfileService, VRUIUtilsService, VR_AccountBalance_BillingTransactionAPIService, WhS_BE_FinancialAccountAPIService) {
        
        function registerDrillDownToCarrierAccount() {
            var drillDownDefinition = {};

            drillDownDefinition.title = "Recurring Charge";
            drillDownDefinition.directive = "vr-genericdata-genericbusinessentity-management";
            /*drillDownDefinition.haspermission = function () {
                return WhS_BE_FinancialAccountAPIService.HasViewFinancialAccountPermission();
            };*/
            drillDownDefinition.loadDirective = function (directiveAPI, carrierAccountItem) {
                carrierAccountItem.genericBusinessGridAPI = directiveAPI;
                var payload = {
                    query: {
                        CarrierAccountId: carrierAccountItem.Entity.CarrierAccountId
                    },
                    carrierAccountId: carrierAccountItem.Entity.CarrierAccountId
                };

                return carrierAccountItem.genericBusinessGridAPI.load(payload);
            };
            WhS_BE_CarrierAccountService.addDrillDownDefinition(drillDownDefinition);
        }

        function registerDrillDownToCarrierProfile() {
            var drillDownDefinition = {};

            drillDownDefinition.title = "Recurring Charge";
            drillDownDefinition.directive = "vr-genericdata-genericbusinessentity-management";
           /* drillDownDefinition.haspermission = function () {
                return WhS_BE_FinancialAccountAPIService.HasViewFinancialAccountPermission();
            };*/
            drillDownDefinition.loadDirective = function (directiveAPI, carrierProfileItem) {
                carrierProfileItem.genericBusinessGridAPI = directiveAPI;
                var payload = {
                    query: {
                        CarrierProfileId: carrierProfileItem.Entity.CarrierProfileId
                    },
                    carrierProfileId: carrierProfileItem.Entity.CarrierProfileId
                };

                return carrierProfileItem.genericBusinessGridAPI.load(payload);
            };

            WhS_BE_CarrierProfileService.addDrillDownDefinition(drillDownDefinition);
        }


        //function defineRecurringChargeDrillDownTabs(financialAccount, gridAPI) {
        //    var drillDownTabs = [];
        //    addBillingTransactionDrillDownTab();

        //    setDrillDownTabs();

        ///*    function addBillingTransactionDrillDownTab() {
        //        if (financialAccount == undefined || financialAccount.BalanceAccountTypeId == undefined)
        //            return;

        //        var drillDownTab = {};

        //        drillDownTab.title = "Financial Transactions";
        //        drillDownTab.directive = "vr-accountbalance-billingtransaction-search";
        //        drillDownTab.haspermission = function () {
        //            return VR_AccountBalance_BillingTransactionAPIService.HasViewBillingTransactionPermission(financialAccount.BalanceAccountTypeId);
        //        };
        //        drillDownTab.loadDirective = function (billingTransactionSearchAPI, financialAccountObj) {
        //            financialAccountObj.billingTransactionSearchAPI = billingTransactionSearchAPI;

        //            var financialAccountId = financialAccount.Entity.FinancialAccountId;
        //            var accountTypeId = financialAccount.BalanceAccountTypeId;

        //            var billingTransactionSearchPayload = {
        //                AccountTypeId: accountTypeId,
        //                AccountsIds: [financialAccountId]
        //            };
        //            return billingTransactionSearchAPI.loadDirective(billingTransactionSearchPayload);
        //        };

        //        drillDownTabs.push(drillDownTab);
        //    }
        //    */
        //    function setDrillDownTabs() {
        //        var drillDownManager = VRUIUtilsService.defineGridDrillDownTabs(drillDownTabs, gridAPI);
        //        drillDownManager.setDrillDownExtensionObject(financialAccount);
        //    }
        //}

        return {
            registerDrillDownToCarrierAccount: registerDrillDownToCarrierAccount,
            registerDrillDownToCarrierProfile: registerDrillDownToCarrierProfile,
           // defineRecurringChargeDrillDownTabs: defineRecurringChargeDrillDownTabs
        };
    }

    appControllers.service('WhS_BE_RecurringChargeService', RecurringChargeService);

})(appControllers);