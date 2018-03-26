(function (appControllers) {

    'use stict';

    FinancialAccountService.$inject = ['VRModalService', 'VRNotificationService', 'WhS_BE_CarrierAccountService', 'WhS_BE_CarrierProfileService', 'VRUIUtilsService', 'VR_AccountBalance_BillingTransactionAPIService', 'WhS_BE_FinancialAccountAPIService'];

    function FinancialAccountService(VRModalService, VRNotificationService, WhS_BE_CarrierAccountService, WhS_BE_CarrierProfileService, VRUIUtilsService, VR_AccountBalance_BillingTransactionAPIService, WhS_BE_FinancialAccountAPIService) {

        function addFinancialAccount(carrierAccountId, carrierProfileId, onFinancialAccountAdded) {
            var parameters = {
                carrierAccountId: carrierAccountId,
                carrierProfileId: carrierProfileId
            };

            var settings = {};
            settings.onScopeReady = function (modalScope) {
                modalScope.onFinancialAccountAdded = onFinancialAccountAdded;
            };
            VRModalService.showModal('/Client/Modules/WhS_BusinessEntity/Views/FinancialAccount/FinancialAccountEditor.html', parameters, settings);
        }

        function editFinancialAccount(onFinancialAccountUpdated, financialAccountId) {
            var parameters = {
                financialAccountId: financialAccountId,
            };

            var settings = {};
            settings.onScopeReady = function (modalScope) {
                modalScope.onFinancialAccountUpdated = onFinancialAccountUpdated;
            };
            VRModalService.showModal('/Client/Modules/WhS_BusinessEntity/Views/FinancialAccount/FinancialAccountEditor.html', parameters, settings);
        }

        function registerDrillDownToCarrierAccount() {
            var drillDownDefinition = {};

            drillDownDefinition.title = "Financial Accounts";
            drillDownDefinition.directive = "whs-be-financialaccount-search";
            drillDownDefinition.haspermission = function () {
                return WhS_BE_FinancialAccountAPIService.HasViewFinancialAccountPermission();
            };
            drillDownDefinition.loadDirective = function (directiveAPI, carrierAccountItem) {
                carrierAccountItem.financialAccountGridAPI = directiveAPI;
                var payload = {
                    query: {
                        CarrierAccountId: carrierAccountItem.Entity.CarrierAccountId
                    },
                    carrierAccountId: carrierAccountItem.Entity.CarrierAccountId
                };

                return carrierAccountItem.financialAccountGridAPI.load(payload);
            };

            WhS_BE_CarrierAccountService.addDrillDownDefinition(drillDownDefinition);
        }

        function registerDrillDownToCarrierProfile() {
            var drillDownDefinition = {};

            drillDownDefinition.title = "Financial Accounts";
            drillDownDefinition.directive = "whs-be-financialaccount-search";
            drillDownDefinition.haspermission = function () {
                return WhS_BE_FinancialAccountAPIService.HasViewFinancialAccountPermission();
            };
            drillDownDefinition.loadDirective = function (directiveAPI, carrierProfileItem) {
                carrierProfileItem.financialAccountGridAPI = directiveAPI;
                var payload = {
                    query: {
                        CarrierProfileId: carrierProfileItem.Entity.CarrierProfileId
                    },
                    carrierProfileId: carrierProfileItem.Entity.CarrierProfileId
                };

                return carrierProfileItem.financialAccountGridAPI.load(payload);
            };

            WhS_BE_CarrierProfileService.addDrillDownDefinition(drillDownDefinition);
        }


        function defineFinancialAccountDrillDownTabs(financialAccount, gridAPI) {
            var drillDownTabs = [];
            addBillingTransactionDrillDownTab();

            setDrillDownTabs();

            function addBillingTransactionDrillDownTab() {
                if (financialAccount == undefined || financialAccount.BalanceAccountTypeId == undefined)
                    return;

                var drillDownTab = {};

                drillDownTab.title = "Financial Transactions";
                drillDownTab.directive = "vr-accountbalance-billingtransaction-search";
                drillDownTab.haspermission = function () {
                    return VR_AccountBalance_BillingTransactionAPIService.HasViewBillingTransactionPermission(financialAccount.BalanceAccountTypeId);
                };
                drillDownTab.loadDirective = function (billingTransactionSearchAPI, financialAccountObj) {
                    financialAccountObj.billingTransactionSearchAPI = billingTransactionSearchAPI;

                    var financialAccountId = financialAccount.Entity.FinancialAccountId;
                    var accountTypeId = financialAccount.BalanceAccountTypeId;

                    var billingTransactionSearchPayload = {
                        AccountTypeId: accountTypeId,
                        AccountsIds: [financialAccountId]
                    };
                    return billingTransactionSearchAPI.loadDirective(billingTransactionSearchPayload);
                };

                drillDownTabs.push(drillDownTab);
            }
            function setDrillDownTabs() {
                var drillDownManager = VRUIUtilsService.defineGridDrillDownTabs(drillDownTabs, gridAPI);
                drillDownManager.setDrillDownExtensionObject(financialAccount);
            }
        }

        return {
            addFinancialAccount: addFinancialAccount,
            editFinancialAccount: editFinancialAccount,
            registerDrillDownToCarrierAccount: registerDrillDownToCarrierAccount,
            registerDrillDownToCarrierProfile: registerDrillDownToCarrierProfile,
            defineFinancialAccountDrillDownTabs: defineFinancialAccountDrillDownTabs
        };
    }

    appControllers.service('WhS_BE_FinancialAccountService', FinancialAccountService);

})(appControllers);