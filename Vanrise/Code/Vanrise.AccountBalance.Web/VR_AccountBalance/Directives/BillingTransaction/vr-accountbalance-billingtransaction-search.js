"use strict";

app.directive("vrAccountbalanceBillingtransactionSearch", ['VRNotificationService', 'UtilsService', 'VRUIUtilsService', 'VRValidationService', 'VR_AccountBalance_LiveBalanceAPIService', 'VR_AccountBalance_BillingTransactionService','VR_AccountBalance_BillingTransactionAPIService','PeriodEnum',
function (VRNotificationService, UtilsService, VRUIUtilsService, VRValidationService, VR_AccountBalance_LiveBalanceAPIService, VR_AccountBalance_BillingTransactionService, VR_AccountBalance_BillingTransactionAPIService, PeriodEnum) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var billingTransactionSearch = new BillingTransactionSearch($scope, ctrl, $attrs);
            billingTransactionSearch.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/VR_AccountBalance/Directives/BillingTransaction/Templates/BillingTransactionSearch.html"

    };

    function BillingTransactionSearch($scope, ctrl, $attrs) {

        var gridAPI;
        var gridPromiseDeferred = UtilsService.createPromiseDeferred();

        var accountTypeId;
        var accountsIds;

        var transactionTypeSelectorAPI;
        var transactionTypeSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var timeRangeDirectiveAPI;
        var timeRangeReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        this.initializeController = initializeController;

        function initializeController() {
            $scope.scopeModel = {};

            $scope.scopeModel.onTimeRangeDirectiveReady = function (api) {
                timeRangeDirectiveAPI = api;
                timeRangeReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.onTransactionTypeSelectorReady = function (api) {
                transactionTypeSelectorAPI = api;
                transactionTypeSelectorReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.hasAddTransaction = false;

            $scope.scopeModel.searchClicked = function () {
                var payload = {
                    query: getFilterObject(),
                    showAccount: false,
                };
                var promises = [];
                promises.push(getCurrentAccountBalance());
                promises.push(gridAPI.loadGrid(payload));

                return UtilsService.waitMultiplePromises(promises);
            };

            $scope.scopeModel.addTransaction = function () {
                var onBillingTransacationAdded = function (obj) {
                    gridAPI.onBillingTransactionAdded(obj);
                };
                VR_AccountBalance_BillingTransactionService.addBillingTransaction(accountsIds[0], accountTypeId, onBillingTransacationAdded)
            };

            $scope.scopeModel.validateDateTime = function () {
                return VRValidationService.validateTimeRange($scope.scopeModel.fromTime, $scope.scopeModel.toTime);
            };

            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
                gridPromiseDeferred.resolve();
            };

            defineAPI();
        }

        function defineAPI()
        {
            var api = {};
            api.loadDirective = function (payload) {
                $scope.scopeModel.isLoading = true;
                var promises = [];
                if (payload != undefined) {
                    accountTypeId = payload.AccountTypeId;
                    accountsIds = payload.AccountsIds;
                  
                    promises.push(getCurrentAccountBalance());
                    promises.push(loadTransactionTypeSelector());
                   
                }
                if (accountTypeId) {
                    promises.push(checkHasAddBillingTransaction(accountTypeId));
                }
                  
                promises.push(loadTimeRangeDirective());
                return UtilsService.waitMultiplePromises(promises).finally(function () {
                    loadBillingTransactionsGrid();
                    $scope.scopeModel.isLoading = false;
                });
            };

            if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                ctrl.onReady(api);
        }

        function getCurrentAccountBalance()
        {
            return VR_AccountBalance_LiveBalanceAPIService.GetCurrentAccountBalance(accountsIds[0], accountTypeId).then(function (response) {
                if (response) {
                    $scope.scopeModel.balance = response.CurrentBalance;
                    $scope.scopeModel.currency = response.CurrencyDescription;
                    $scope.scopeModel.balanceFlagDescription = response.BalanceFlagDescription;
                }
            });
        }

        function loadTransactionTypeSelector() {
            var transactionTypeSelectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();
            transactionTypeSelectorReadyPromiseDeferred.promise.then(function () {
                var transactionTypePayload = {
                    filter:{
                        AccountTypeId: accountTypeId
                    }
                };
                VRUIUtilsService.callDirectiveLoad(transactionTypeSelectorAPI, transactionTypePayload, transactionTypeSelectorLoadPromiseDeferred);
            });
            return transactionTypeSelectorLoadPromiseDeferred.promise;
        }

        function loadTimeRangeDirective() {
            var loadTimeDimentionPromiseDeferred = UtilsService.createPromiseDeferred();
            timeRangeReadyPromiseDeferred.promise.then(function () {
                var timeRangePeriod = {
                    period: PeriodEnum.CurrentYear.value
                };

                VRUIUtilsService.callDirectiveLoad(timeRangeDirectiveAPI, timeRangePeriod, loadTimeDimentionPromiseDeferred);

            });
            return loadTimeDimentionPromiseDeferred.promise;
        }

        function getFilterObject() {
            var filter = {
                TransactionTypeIds:transactionTypeSelectorAPI.getSelectedIds(),
                AccountTypeId: accountTypeId,
                AccountsIds: accountsIds,
                FromTime: $scope.scopeModel.fromTime,
                ToTime: $scope.scopeModel.toTime,
            };
            return filter;
        }
        function loadBillingTransactionsGrid()
        {
           return gridPromiseDeferred.promise.then(function () {
                var payload = {
                    query: getFilterObject(),
                    showAccount: false,
                };
                gridAPI.loadGrid(payload);
            });
        }
        function checkHasAddBillingTransaction(accountTypeId) {
          return VR_AccountBalance_BillingTransactionAPIService.HasAddBillingTransactionPermission(accountTypeId).then(function (response) {
                $scope.scopeModel.hasAddTransaction = response;
            });
        }
    }

    return directiveDefinitionObject;

}]);
