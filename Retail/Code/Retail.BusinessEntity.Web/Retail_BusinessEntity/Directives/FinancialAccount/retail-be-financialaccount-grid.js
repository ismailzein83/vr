'use strict';

app.directive('retailBeFinancialaccountGrid', ['Retail_BE_FinancialAccountService', 'Retail_BE_FinancialAccountAPIService', 'VRNotificationService', 'VRUIUtilsService', 'Retail_BE_AccountBalanceTypeAPIService','UtilsService', function (Retail_BE_FinancialAccountService, Retail_BE_FinancialAccountAPIService, VRNotificationService, VRUIUtilsService, Retail_BE_AccountBalanceTypeAPIService, UtilsService) {
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
                var gridDrillDownTabDefinitions = getGridDrillDownTabDefinitions();
                gridDrillDownTabManager = VRUIUtilsService.defineGridDrillDownTabs(gridDrillDownTabDefinitions, gridAPI, undefined);
                defineAPI();
            };

            $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return Retail_BE_FinancialAccountAPIService.GetFilteredFinancialAccounts(dataRetrievalInput).then(function (response) {
                    if (response != undefined && response.Data != null) {
                        for (var i = 0; i < response.Data.length; i++) {
                            var financialAccount = response.Data[i];
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
                var promises = [];
                promises.push(gridAPI.retrieveData(payload.query));
                return UtilsService.waitMultiplePromises(promises);
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
                clicked: editFinancialAccount,
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
            Retail_BE_FinancialAccountService.editFinancialAccount(onFinancialAccountUpdated,accountBEDefinitionId,accountId, financialAccount.SequenceNumber);
        }

        function getGridDrillDownTabDefinitions() {
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

            return drillDownTabDefinitions;
        }
    }
}]);
