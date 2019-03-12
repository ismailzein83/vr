'use strict';

app.directive('retailBeFinancialaccountGrid', ['Retail_BE_FinancialAccountService', 'Retail_BE_FinancialAccountAPIService', 'VRNotificationService', 'VRUIUtilsService', 'Retail_BE_AccountBalanceTypeAPIService', 'UtilsService', 'Retail_BE_FinacialRecurringChargeAPIService', 'Retail_BE_AccountBEDefinitionAPIService', 'VR_GenericData_GenericBusinessEntityAPIService',
    function (Retail_BE_FinancialAccountService, Retail_BE_FinancialAccountAPIService, VRNotificationService, VRUIUtilsService, Retail_BE_AccountBalanceTypeAPIService, UtilsService, Retail_BE_FinacialRecurringChargeAPIService, Retail_BE_AccountBEDefinitionAPIService, VR_GenericData_GenericBusinessEntityAPIService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var financialAccountGrid = new FinancialAccountGrid($scope, ctrl, $attrs);
                financialAccountGrid.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/FinancialAccount/Templates/FinancialAccountGridTemplate.html'
        };

        function FinancialAccountGrid($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var gridAPI;
            var accountBEDefinitionId;
            var accountId;
            var context;
            var gridDrillDownTabManager;
            var accountTypeId;
            var useRecurringChargeModule;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.financialAccounts = [];
                $scope.scopeModel.menuActions = [];
                $scope.scopeModel.showFinancialTransactionGrid = function (dataItem) {
                    if (dataItem.BalanceAccountTypeId != undefined)
                        return true;
                    return false;
                };
                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return Retail_BE_FinancialAccountAPIService.GetFilteredFinancialAccounts(dataRetrievalInput).then(function (response) {
                        if (response != undefined && response.Data != null) {
                            for (var i = 0; i < response.Data.length; i++) {
                                var financialAccount = response.Data[i];

                                var gridDrillDownTabDefinitions = getGridDrillDownTabDefinitions(financialAccount);
                                gridDrillDownTabManager = VRUIUtilsService.defineGridDrillDownTabs(gridDrillDownTabDefinitions, gridAPI, undefined);

                                if (financialAccount.BalanceAccountTypeId != undefined)
                                    gridDrillDownTabManager.setDrillDownExtensionObject(financialAccount);
                            }
                        }
                        onResponseReady(response);
                    }).catch(function (error) {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                    });
                };

                defineMenuActions();
            }
            function defineAPI() {
                var api = {};

                api.loadGrid = function (payload) {
                    accountBEDefinitionId = payload.accountBEDefinitionId;
                    context = payload.context;
                    accountId = payload.accountId;


                    function getRecurringChargeModulePromise() {
                        return Retail_BE_AccountBEDefinitionAPIService.CheckUseRecurringChargeModule(accountBEDefinitionId).then(function (response) {
                            useRecurringChargeModule = response;
                        });
                    }
                    var rootPromiseNode = {
                        promises: [getRecurringChargeModulePromise()],
                        getChildNode: function () {
                            return {
                                promises: [gridAPI.retrieveData(payload.query)]
                            };

                        }
                    };
                    return UtilsService.waitPromiseNode(rootPromiseNode);
                };

                api.onFinancialAccountAdded = function (addedFinancialAccount) {
                    gridAPI.itemAdded(addedFinancialAccount);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function defineMenuActions() {
                $scope.scopeModel.menuActions.push({
                    name: 'Edit',
                    clicked: editFinancialAccount
                });
            }

            function editFinancialAccount(financialAccount) {
                var onFinancialAccountUpdated = function (updatedFinancialAccount) {
                    if (context != undefined && context.checkAllowAddFinancialAccount != undefined)
                        context.checkAllowAddFinancialAccount();
                    if (updatedFinancialAccount.BalanceAccountTypeId != undefined)
                        gridDrillDownTabManager.setDrillDownExtensionObject(financialAccount);
                    gridDrillDownTabManager.setDrillDownExtensionObject(updatedFinancialAccount);
                    gridAPI.itemUpdated(updatedFinancialAccount);
                };
                Retail_BE_FinancialAccountService.editFinancialAccount(onFinancialAccountUpdated, accountBEDefinitionId, accountId, financialAccount.SequenceNumber);
            }

            function getGridDrillDownTabDefinitions(financialAccount) {
                var drillDownTabDefinitions = [];
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

                drillDownTabDefinitions.push(transactionTab);
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
                    return genericBusinessEntityAPI.load(genericBusinessEntityPayload);
                };
                drillDownTabDefinitions.push(recurringChargeDrillDownTab);
            }
            return drillDownTabDefinitions;
        }
    }
}]);
