(function (appControllers) {

    'use strict';

    FinancialAccountService.$inject = ['UtilsService', 'VRModalService', 'VRUIUtilsService', 'VR_GenericData_GenericBusinessEntityAPIService','Retail_BE_FinancialAccountAPIService'];

    function FinancialAccountService(UtilsService, VRModalService, VRUIUtilsService, VR_GenericData_GenericBusinessEntityAPIService, Retail_BE_FinancialAccountAPIService) {
        return ({
            addFinancialAccount: addFinancialAccount,
            editFinancialAccount: editFinancialAccount,
            defineFinancialAccountTabs: defineFinancialAccountTabs
        });

        function addFinancialAccount(onFinancialAccountAdded, accountBEDefinitionId, accountId) {
            var modalParameters = {
                accountBEDefinitionId: accountBEDefinitionId,
                accountId: accountId
            };
            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onFinancialAccountAdded = onFinancialAccountAdded;
            };

            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Views/FinancialAccount/FinancialAccountEditor.html', modalParameters, modalSettings);
        }

        function editFinancialAccount(onFinancialAccountUpdated, accountBEDefinitionId,accountId, sequenceNumber) {
            var modalParameters = {
                accountBEDefinitionId:accountBEDefinitionId,
                sequenceNumber: sequenceNumber,
                accountId: accountId
            };
            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onFinancialAccountUpdated = onFinancialAccountUpdated;
            };

            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Views/FinancialAccount/FinancialAccountEditor.html', modalParameters, modalSettings);
        }

        function defineFinancialAccountTabs(financialAccount, gridAPI, useRecurringChargeModule, accountBEDefinitionId) {
            if (financialAccount == undefined || gridAPI == undefined)
                return;

            var drillDownTabs = [];
            addTransactionDrillDownTab();
            addRecurringChargeDrillDownTabs();
            setDrillDownTabs();

            function addTransactionDrillDownTab() {
                var transactionTab = {
                    title: "Financial Transactions",
                    directive: "vr-accountbalance-billingtransaction-search",
                    loadDirective: function (billingTransactionSearchAPI, financialAccount) {
                        financialAccount.billingTransactionSearchAPI = billingTransactionSearchAPI;
                        var billingTransactionSearchPayload = {
                            accountBEDefinitionId: accountBEDefinitionId,
                            AccountsIds: [financialAccount.FinancialAccountId],
                            AccountTypeId: financialAccount.BalanceAccountTypeId
                        };
                        return billingTransactionSearchAPI.loadDirective(billingTransactionSearchPayload);
                    }
                };
                if (financialAccount.BalanceAccountTypeId != undefined)
                   drillDownTabs.push(transactionTab);
            }
            function addRecurringChargeDrillDownTabs() {
                if (financialAccount.Classifications != undefined && financialAccount.Classifications.length > 0 && useRecurringChargeModule) {
                    for (var i = 0; i < financialAccount.Classifications.length; i++) {
                        var classification = financialAccount.Classifications[i];
                        defineRecurringChargeDrillDownTabs(classification);
                    }
                }
                function defineRecurringChargeDrillDownTabs(classification) {
                    var recurringChargeDrillDownTab = {};

                    recurringChargeDrillDownTab.title = classification + " Recurring Charges";
                    recurringChargeDrillDownTab.directive = "vr-genericdata-genericbusinessentity-management";
                    recurringChargeDrillDownTab.haspermission = function () {
                        var businessEntityDefinitionId = "dd2cbb22-0fc8-4ad2-bdcd-cb63a3e5dea8";
                        return VR_GenericData_GenericBusinessEntityAPIService.DoesUserHaveViewAccess(businessEntityDefinitionId);
                    };
                    recurringChargeDrillDownTab.loadDirective = function (genericBusinessEntityAPI, financialAccount) {
                        var financialAccountId = financialAccount.FinancialAccountId;
                        var currencyId;
                        function getAccountCurrency() {
                            return Retail_BE_FinancialAccountAPIService.GetAccountCurrency(accountBEDefinitionId, financialAccountId).then(function (response) {
                                currencyId = response;
                            });
                        }
                        var rootPromiseNode = {
                            promises: [getAccountCurrency()],
                            getChildNode: function () {
                                var genericBusinessEntityPayload = {
                                    businessEntityDefinitionId: "DD2CBB22-0FC8-4AD2-BDCD-CB63A3E5DEA8",
                                    fieldValues: {
                                        FinancialAccountId: {
                                            value: financialAccountId,
                                            isHidden: true,
                                            isDisabled: false
                                        },
                                        Classification: {
                                            value: classification,
                                            isHidden: true,
                                            isDisabled: false
                                        },
                                        CurrencyId: {
                                            value: currencyId,
                                            isDisabled: false,
                                            isHidden: false
                                        }
                                    },
                                    filterValues: {
                                        FinancialAccountId: {
                                            value: financialAccountId,
                                            isHidden: true,
                                            isDisabled: false
                                        },
                                        Classification: {
                                            value: classification,
                                            isHidden: true,
                                            isDisabled: false
                                        }
                                    }
                                };
                                return {
                                    promises: [genericBusinessEntityAPI.load(genericBusinessEntityPayload)]
                                };
                            }
                        };
                        return UtilsService.waitPromiseNode(rootPromiseNode);
                    };
                    drillDownTabs.push(recurringChargeDrillDownTab);
                }
            }

            function setDrillDownTabs() {
                var drillDownManager = VRUIUtilsService.defineGridDrillDownTabs(drillDownTabs, gridAPI);
                drillDownManager.setDrillDownExtensionObject(financialAccount);
            }

        }
    }

    appControllers.service('Retail_BE_FinancialAccountService', FinancialAccountService);

})(appControllers);
