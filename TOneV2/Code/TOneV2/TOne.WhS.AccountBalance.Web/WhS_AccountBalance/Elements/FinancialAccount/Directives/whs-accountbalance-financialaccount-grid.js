'use strict';

app.directive('whsAccountbalanceFinancialaccountGrid', ['WhS_AccountBalance_FinancialAccountAPIService', 'VRNotificationService', 'VR_AccountBalance_FinancialAccountService', 'VRUIUtilsService', function (WhS_AccountBalance_FinancialAccountAPIService, VRNotificationService, VR_AccountBalance_FinancialAccountService, VRUIUtilsService) {
    return {
        restrict: 'E',
        scope: {
            onReady: '=',
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            ctrl.showAccount = true;
            var financialAccountGrid = new FinancialAccountGrid($scope, ctrl, $attrs);
            financialAccountGrid.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        templateUrl: '/Client/Modules/WhS_AccountBalance/Elements/FinancialAccount/Directives/Templates/FinancialAccountGridTemplate.html'
    };

    function FinancialAccountGrid($scope, ctrl, $attrs) {

        this.initializeController = initializeController;

        var context;

        var gridAPI;
        var gridDrillDownTabManager;

        function initializeController() {
            $scope.scopeModel = {};
            $scope.scopeModel.financialAccounts = [];
            $scope.scopeModel.menuActions = [];

            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
                var gridDrillDownTabDefinitions = getGridDrillDownTabDefinitions();
                gridDrillDownTabManager = VRUIUtilsService.defineGridDrillDownTabs(gridDrillDownTabDefinitions, gridAPI, undefined);
                defineAPI();
            };

            $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return WhS_AccountBalance_FinancialAccountAPIService.GetFilteredFinancialAccounts(dataRetrievalInput).then(function (response) {
                    if (response != undefined && response.Data != null) {
                        for (var i = 0; i < response.Data.length; i++) {
                            var financialAccount = response.Data[i];
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

                var query;

                if (payload != undefined) {
                    context = payload.context;
                    query = payload.query;
                }

                return gridAPI.retrieveData(query);
            };

            api.onFinancialAccountAdded = function (financialAccount) {
                gridDrillDownTabManager.setDrillDownExtensionObject(financialAccount);
                return gridAPI.itemAdded(financialAccount);
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        function getGridDrillDownTabDefinitions() {
            var drillDownTabDefinitions = [];

            var transactionTab = {
                title: "Financial Transactions",
                directive: "vr-accountbalance-billingtransaction-search",
                loadDirective: function (billingTransactionSearchAPI, financialAccount) {
                    financialAccount.billingTransactionSearchAPI = billingTransactionSearchAPI;

                    var financialAccountId = financialAccount.Entity.FinancialAccountId;
                    var accountTypeId = (financialAccount.Entity.Settings != null) ? financialAccount.Entity.Settings.AccountTypeId : undefined;

                    var billingTransactionSearchPayload = {
                        AccountTypeId: accountTypeId,
                        AccountsIds: [financialAccountId]
                    };

                    return billingTransactionSearchAPI.loadDirective(billingTransactionSearchPayload);
                }
            };

            drillDownTabDefinitions.push(transactionTab);

            return drillDownTabDefinitions;
        }
        function defineMenuActions() {

            var menuActions = [{
                name: "Edit",
                clicked: editFinancialAccount,
            }];

            $scope.scopeModel.gridMenuActions = function (dataItem) {
                if (dataItem.IsActive) {
                    return menuActions;
                }
                return null;
            };
        }
        function editFinancialAccount(dataItem) {
            var onFinancialAccountUpdated = function (financialAccount) {
                if (context != undefined && context.checkAllowAddFinancialAccount != undefined)
                    context.checkAllowAddFinancialAccount();
                gridAPI.itemUpdated(financialAccount);
            };
            VR_AccountBalance_FinancialAccountService.editFinancialAccount(onFinancialAccountUpdated, dataItem.Entity.FinancialAccountId);
        }
    }
}]);