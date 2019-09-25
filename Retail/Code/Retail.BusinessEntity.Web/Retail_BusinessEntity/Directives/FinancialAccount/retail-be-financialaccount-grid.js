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
                                Retail_BE_FinancialAccountService.defineFinancialAccountTabs(financialAccount, gridAPI, useRecurringChargeModule, accountBEDefinitionId);
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
                    Retail_BE_FinancialAccountService.defineFinancialAccountTabs(addedFinancialAccount, gridAPI, useRecurringChargeModule, accountBEDefinitionId);
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
                    Retail_BE_FinancialAccountService.defineFinancialAccountTabs(updatedFinancialAccount, gridAPI, useRecurringChargeModule, accountBEDefinitionId);
                    gridAPI.itemUpdated(updatedFinancialAccount);
                };
                Retail_BE_FinancialAccountService.editFinancialAccount(onFinancialAccountUpdated, accountBEDefinitionId, accountId, financialAccount.SequenceNumber);
            }
    }
}]);
