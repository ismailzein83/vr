(function (app) {

    "use strict";

    accountBalancesManagementController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService', 'VRNavigationService', 'VR_AccountBalance_AccountTypeAPIService', 'VR_AccountBalance_BillingTransactionService', 'VR_AccountBalance_BalanceOrderByEnum'];

    function accountBalancesManagementController($scope, UtilsService, VRUIUtilsService, VRNotificationService, VRNavigationService, VR_AccountBalance_AccountTypeAPIService, VR_AccountBalance_BillingTransactionService, VR_AccountBalance_BalanceOrderByEnum) {
        var gridAPI;
        var accountTypeId;

        var filterDirectiveAPI;
        var filterDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

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
            $scope.top = 1000;
            $scope.signs = [{ text: '>', value: '>' }, { text: '=>', value: '>=' }, { text: '<', value: '<' }, { text: '<=', value: '<=' }];
            $scope.orderByOptions = UtilsService.getArrayEnum(VR_AccountBalance_BalanceOrderByEnum);
            $scope.orderBy = $scope.orderByOptions[0];
            $scope.onGridReady = function (api) {
                gridAPI = api;
            };
            $scope.searchClicked = function (api) {           
                gridAPI.loadGrid(getFilterObject());
            };
            $scope.onSignSelectionChanged = function () {
                if (!$scope.sign)
                    $scope.balance = null;
            };
            $scope.onFilterDirectiveReady = function (api) {
                filterDirectiveAPI = api;
                filterDirectiveReadyDeferred.resolve();
            };
        }



        function load() {
            $scope.isLoading = true;
            VR_AccountBalance_AccountTypeAPIService.GetAccountSelector(accountTypeId).then(function (response) {
                $scope.filterEditor = response;
                loadAllControls();
            });
        }

        function loadAllControls() {           
            return UtilsService.waitMultipleAsyncOperations([loadFilterDirective]).catch(function (error) {
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
        function getFilterObject() {
            return {
                AccountTypeId: accountTypeId,
                Top: $scope.top,
                AccountsIds: (filterDirectiveAPI != undefined) ? filterDirectiveAPI.getData().selectedIds : null,
                Sign: $scope.sign!=undefined ? $scope.sign.value: undefined ,
                Balance: $scope.sign != undefined ? $scope.balance : undefined,
                OrderBy:$scope.orderBy.value
            };
        }
    }

    app.controller('VR_AccountBalance_AccountBalancesManagementController', accountBalancesManagementController);
})(app);