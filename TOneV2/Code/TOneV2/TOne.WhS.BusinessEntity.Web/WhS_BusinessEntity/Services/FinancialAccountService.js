(function (appControllers) {

    'use stict';

    FinancialAccountService.$inject = ['VRModalService', 'VRNotificationService', 'WhS_BE_CarrierAccountService', 'WhS_BE_CarrierProfileService', 'UtilsService', 'VRUIUtilsService', 'VR_AccountBalance_BillingTransactionAPIService', 'WhS_BE_FinancialAccountAPIService', 'WhS_BE_FinancialAccountDefinitionAPIService', 'WhS_BE_RecurringChargeAPIService', 'WhS_BE_CarrierProfileAPIService', 'WhS_BE_CarrierAccountAPIService'];

    function FinancialAccountService(VRModalService, VRNotificationService, WhS_BE_CarrierAccountService, WhS_BE_CarrierProfileService, UtilsService, VRUIUtilsService, VR_AccountBalance_BillingTransactionAPIService, WhS_BE_FinancialAccountAPIService, WhS_BE_FinancialAccountDefinitionAPIService, WhS_BE_RecurringChargeAPIService, WhS_BE_CarrierProfileAPIService, WhS_BE_CarrierAccountAPIService) {

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

            if (financialAccount.IsApplicableToCustomer)
                addCustomerRecurringChargeDrillDownTab();

            if (financialAccount.IsApplicableToSupplier)
                addSupplierRecurringChargeDrillDownTab();

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

            function addCustomerRecurringChargeDrillDownTab() {
                if (financialAccount == undefined)
                    return;
                var customerDrillDownTab = {};

                customerDrillDownTab.title = "Customer Recurring Charge";
                customerDrillDownTab.directive = "vr-genericdata-genericbusinessentity-management";
                customerDrillDownTab.haspermission = function () {
                    return WhS_BE_RecurringChargeAPIService.HasViewRecurringChargePermission(financialAccount.Entity.FinancialAccountId);
                };
                customerDrillDownTab.loadDirective = function (genericBusinessEntityAPI, financialAccountObj) {
                    var financialAccountId = financialAccountObj.Entity.FinancialAccountId;
                    var promise = UtilsService.createPromiseDeferred();
                    WhS_BE_FinancialAccountAPIService.GetFinancialAccountCurrencyId(financialAccountId).then(function (currencyId) {
                        var genericBusinessEntityPayload = {
                            businessEntityDefinitionId: "fa6c91c0-adc9-4bb2-aedb-77a6ee1c9131",
                            fieldValues: {
                                FinancialAccountId: financialAccountId,
                                CurrencyId: currencyId
                            }
                        };
                        genericBusinessEntityAPI.load(genericBusinessEntityPayload).then(function () {
                            promise.resolve();
                        }).catch(function (error) {
                            promise.reject(error);
                        });
                    }).catch(function (error) {
                        promise.reject(error);
                    });
                    return promise.promise;
                };

                drillDownTabs.push(customerDrillDownTab);
            }

            function addSupplierRecurringChargeDrillDownTab() {
                if (financialAccount == undefined)
                    return;
                var supplierDrillDownTab = {};

                supplierDrillDownTab.title = "Supplier Recurring Charge";
                supplierDrillDownTab.directive = "vr-genericdata-genericbusinessentity-management";
                supplierDrillDownTab.haspermission = function () {
                    return WhS_BE_RecurringChargeAPIService.HasViewRecurringChargePermission(financialAccount.Entity.FinancialAccountId);
                };
                supplierDrillDownTab.loadDirective = function (genericBusinessEntityAPI, financialAccountObj) {
                    var financialAccountId = financialAccountObj.Entity.FinancialAccountId;
                    var promise = UtilsService.createPromiseDeferred();
                    WhS_BE_FinancialAccountAPIService.GetFinancialAccountCurrencyId(financialAccountId).then(function (currencyId) {
                        var genericBusinessEntityPayload = {
                            businessEntityDefinitionId: "e9c11a90-864c-45a1-b90c-d7fdd80e9cf3",
                            fieldValues: {
                                FinancialAccountId: financialAccountId,
                                CurrencyId: currencyId
                            }
                        };
                        genericBusinessEntityAPI.load(genericBusinessEntityPayload).then(function () {
                            promise.resolve();
                        }).catch(function (error) {
                            promise.reject(error);
                        });;
                    }).catch(function (error) {
                        promise.reject(error);
                    });;
                    return promise.promise;
                };

                drillDownTabs.push(supplierDrillDownTab);
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