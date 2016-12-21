(function (app) {

    "use strict";

    billingTransactionManagementController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService', 'VRNavigationService', 'VR_AccountBalance_AccountTypeAPIService', 'VR_AccountBalance_BillingTransactionService'];

    function billingTransactionManagementController($scope, UtilsService, VRUIUtilsService, VRNotificationService, VRNavigationService, VR_AccountBalance_AccountTypeAPIService, VR_AccountBalance_BillingTransactionService) {
        var gridAPI;
        var accountTypeId;

        var filterDirectiveAPI;
        var filterDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

        var transactionTypeDirectiveAPI;
        var transactionTypeDirectiveReadyDeferred = UtilsService.createPromiseDeferred();


        loadParameters();
        defineScope();
        load();

        var filter = {};
        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != null) {
                accountTypeId = parameters.accountTypeId;
            }
        }
        
        function defineScope() {
            var today = new Date();
            today.setMonth(today.getMonth() - 1);
            today.setHours(0, 0, 0, 0);
            $scope.fromTime = today;
            $scope.onGridReady = function (api) {
                gridAPI = api;
            };
            $scope.searchClicked = function (api) {
                var payload = {
                    query: getFilterObject()
                }
                return gridAPI.loadGrid(payload);
            };
            $scope.addClicked = function (api) {
                var onBillingTransactionAdded = function (transactionObj) {
                    if (gridAPI != undefined) {
                        gridAPI.onBillingTransactionAdded(transactionObj);
                    }
                };
                VR_AccountBalance_BillingTransactionService.addBillingTransaction(undefined, accountTypeId, onBillingTransactionAdded);
            };
            $scope.onFilterDirectiveReady = function (api) {
                filterDirectiveAPI = api;
                filterDirectiveReadyDeferred.resolve();
            };
            $scope.onBillingTransactionTypeReady = function (api) {
                transactionTypeDirectiveAPI = api;
                transactionTypeDirectiveReadyDeferred.resolve();
            }
        }



        function load() {
            $scope.isLoading = true;
            VR_AccountBalance_AccountTypeAPIService.GetAccountSelector(accountTypeId).then(function (response) {
                $scope.filterEditor = response;
                loadAllControls();
            });
        }

        function loadAllControls() {           
            return UtilsService.waitMultipleAsyncOperations([loadFilterDirective,loadTransactionTypeSelector]).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });
        }
        function loadFilterDirective() {
            var loadFilterPromiseDeferred = UtilsService.createPromiseDeferred();
            filterDirectiveReadyDeferred.promise.then(function () {               
                VRUIUtilsService.callDirectiveLoad(filterDirectiveAPI, undefined, loadFilterPromiseDeferred);
            });
            return loadFilterPromiseDeferred.promise;
        }
        function loadTransactionTypeSelector() {
            var loadTransactionTypePromiseDeferred = UtilsService.createPromiseDeferred();
            transactionTypeDirectiveReadyDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(transactionTypeDirectiveAPI, undefined, loadTransactionTypePromiseDeferred);
            });
            return loadTransactionTypePromiseDeferred.promise;
        }
        function getFilterObject() {
            return {
                FromTime: $scope.fromTime,
                ToTime: $scope.toTime,
                AccountTypeId: accountTypeId,
                TransactionTypeIds: transactionTypeDirectiveAPI.getSelectedIds(),
                AccountsIds:(filterDirectiveAPI!=undefined)? filterDirectiveAPI.getData().selectedIds : null
            };
        }
    }

    app.controller('VR_AccountBalance_BillingTransactionManagementController', billingTransactionManagementController);
})(app);