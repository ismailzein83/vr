(function (app) {

    "use strict";

    accountBalancesManagementController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService', 'VRNavigationService', 'VR_AccountBalance_AccountTypeAPIService', 'VR_AccountBalance_BillingTransactionService', 'VR_AccountBalance_BalanceOrderByEnum'];

    function accountBalancesManagementController($scope, UtilsService, VRUIUtilsService, VRNotificationService, VRNavigationService, VR_AccountBalance_AccountTypeAPIService, VR_AccountBalance_BillingTransactionService, VR_AccountBalance_BalanceOrderByEnum) {
        var gridAPI;
        var viewId;



        var accountDirectiveAPI;
        var accountDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

        var accountTypeAPI;
        var accountTypeReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        var filter = {};
        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != null) {
                viewId = parameters.viewId;
            }
        }


        
        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.gridloadded = false;
            $scope.scopeModel.top = 1000;
            $scope.scopeModel.signs = [{ text: '>', value: '>' }, { text: '=>', value: '>=' }, { text: '<', value: '<' }, { text: '<=', value: '<=' }];
            $scope.scopeModel.orderByOptions = UtilsService.getArrayEnum(VR_AccountBalance_BalanceOrderByEnum);
            $scope.scopeModel.orderBy = $scope.scopeModel.orderByOptions[0];

            $scope.scopeModel.onAccountTypeSelectorReady = function (api) {
                accountTypeAPI = api;
                accountTypeReadyDeferred.resolve();
            };

            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
            };
            $scope.scopeModel.searchClicked = function (api) {           
               return  gridAPI.loadGrid(getFilterObject());
            };
            $scope.scopeModel.onSignSelectionChanged = function () {
                if (!$scope.scopeModel.sign)
                    $scope.scopeModel.balance = null;
            };
            $scope.scopeModel.onAccountDirectiveReady = function (api) {
                accountDirectiveAPI = api;
                accountDirectiveReadyDeferred.resolve();
            };

            $scope.scopeModel.onAccountTypeSelectorSelectionChange = function () {
                if (accountTypeAPI.getSelectedIds() != undefined) {
                    $scope.scopeModel.gridloadded = false;
                    loadAllControls().then(function () {
                        $scope.scopeModel.gridloadded = true;
                    });
                }
            };


        }
        function load() {
            $scope.scopeModel.isLoading = true;
            loadAccountType();
        }


        function loadAccountType() {
            var loadAccountTypeSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
            accountTypeReadyDeferred.promise.then(function () {
                var payLoad;
                payLoad = {
                    filter: {
                        Filters: [{
                            $type: "Vanrise.AccountBalance.Business.AccountTypeViewFilter, Vanrise.AccountBalance.Business",
                            ViewId: viewId
                        }]
                    },
                    selectfirstitem: true
                }
                VRUIUtilsService.callDirectiveLoad(accountTypeAPI, payLoad, loadAccountTypeSelectorPromiseDeferred);
            });
            return loadAccountTypeSelectorPromiseDeferred.promise.then(function () {
                $scope.scopeModel.isLoading = false;
                $scope.scopeModel.hideAccountType = accountTypeAPI.hasSingleItem();
            });
        }


        

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadAccountDirective]).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
        function loadAccountDirective() {
            var loadAccountPromiseDeferred = UtilsService.createPromiseDeferred();
            accountDirectiveReadyDeferred.promise.then(function () {
                var payload = {
                    accountTypeId: accountTypeAPI.getSelectedIds()
                };
                VRUIUtilsService.callDirectiveLoad(accountDirectiveAPI, payload, loadAccountPromiseDeferred);
            });
            return loadAccountPromiseDeferred.promise;
        }
        function getFilterObject() {
            return {
                AccountTypeId: accountTypeAPI.getSelectedIds(),
                Top: $scope.scopeModel.top,
                AccountsIds: (accountDirectiveAPI != undefined) ? accountDirectiveAPI.getData().selectedIds : null,
                Sign: $scope.scopeModel.sign!=undefined ? $scope.scopeModel.sign.value: undefined ,
                Balance: $scope.scopeModel.sign != undefined ? $scope.scopeModel.balance : undefined,
                OrderBy:$scope.scopeModel.orderBy.value
            };
        }
    }

    app.controller('VR_AccountBalance_AccountBalancesManagementController', accountBalancesManagementController);
})(app);