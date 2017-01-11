(function (app) {

    "use strict";

    billingTransactionManagementController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService', 'VRNavigationService', 'VR_AccountBalance_AccountTypeAPIService', 'VR_AccountBalance_BillingTransactionService', 'VR_AccountBalance_BillingTransactionAPIService', 'VRValidationService'];

    function billingTransactionManagementController($scope, UtilsService, VRUIUtilsService, VRNotificationService, VRNavigationService, VR_AccountBalance_AccountTypeAPIService, VR_AccountBalance_BillingTransactionService, VR_AccountBalance_BillingTransactionAPIService, VRValidationService) {
        var gridAPI;
        var viewId;

    
        var accountTypeAPI;
        var accountTypeReadyDeferred = UtilsService.createPromiseDeferred();

        var accountDirectiveAPI;
        var accountDirectiveReadyDeferred = UtilsService.createPromiseDeferred();


        var transactionTypeDirectiveAPI;
        var transactionTypeDirectiveReadyDeferred = UtilsService.createPromiseDeferred();


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
            var today = new Date();
            today.setMonth(today.getMonth() - 1);
            today.setHours(0, 0, 0, 0);
            $scope.scopeModel.fromTime = today;

            $scope.scopeModel.onAccountTypeSelectorReady = function (api) {
                accountTypeAPI = api;
                accountTypeReadyDeferred.resolve();
            };
            $scope.scopeModel.onAccountTypeSelectorSelectionChange = function () {
                if (accountTypeAPI.getSelectedIds() != undefined) {
                    $scope.scopeModel.gridloadded = false;
                    loadAllControls().then(function () {
                        $scope.scopeModel.gridloadded = true;
                    });
                }
            };
            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
            };
            $scope.scopeModel.onAccountDirectiveReady = function (api) {
                accountDirectiveAPI = api;
                accountDirectiveReadyDeferred.resolve();
            };
            $scope.scopeModel.searchClicked = function (api) {
                var payload = {
                    query: getFilterObject()
                }
                return gridAPI.loadGrid(payload);
            };
            $scope.validateDateTime = function () {
                return VRValidationService.validateTimeRange($scope.scopeModel.fromTime, $scope.scopeModel.toTime);
            };
            $scope.scopeModel.addClicked = function (api) {
                var onBillingTransactionAdded = function (transactionObj) {
                    if (gridAPI != undefined) {
                        gridAPI.onBillingTransactionAdded(transactionObj);
                    }
                };
                VR_AccountBalance_BillingTransactionService.addBillingTransaction(undefined, accountTypeAPI.getSelectedIds(), onBillingTransactionAdded);
            };
           
            $scope.scopeModel.onBillingTransactionTypeReady = function (api) {
                transactionTypeDirectiveAPI = api;
                transactionTypeDirectiveReadyDeferred.resolve();
            };
            $scope.scopeModel.hasAddBillingTransactionPermission = function () {
                return VR_AccountBalance_BillingTransactionAPIService.HasAddBillingTransactionPermission();
            };

        }



        function load() {
            $scope.scopeModel.isLoading = true;
            loadAccountType().then(function () {
                $scope.scopeModel.isLoading = false;
            });
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
            return UtilsService.waitMultipleAsyncOperations([loadAccountDirective, loadTransactionTypeSelector])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
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
        function loadTransactionTypeSelector() {
            var loadTransactionTypePromiseDeferred = UtilsService.createPromiseDeferred();
            transactionTypeDirectiveReadyDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(transactionTypeDirectiveAPI, undefined, loadTransactionTypePromiseDeferred);
            });
            return loadTransactionTypePromiseDeferred.promise;
        }
        function getFilterObject() {
            return {
                FromTime: $scope.scopeModel.fromTime,
                ToTime: $scope.scopeModel.toTime,
                AccountTypeId: accountTypeAPI.getSelectedIds(),
                TransactionTypeIds: transactionTypeDirectiveAPI.getSelectedIds(),
                AccountsIds: (accountDirectiveAPI != undefined) ? accountDirectiveAPI.getData().selectedIds : null
            };
        }
    }

    app.controller('VR_AccountBalance_BillingTransactionManagementController', billingTransactionManagementController);
})(app);