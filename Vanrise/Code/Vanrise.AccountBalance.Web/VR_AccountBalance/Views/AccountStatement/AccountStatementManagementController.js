(function (appControllers) {

    "use strict";

    accountStatementManagementController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VR_AccountBalance_AccountTypeAPIService','VRNotificationService'];

    function accountStatementManagementController($scope, UtilsService, VRUIUtilsService, VRNavigationService, VR_AccountBalance_AccountTypeAPIService, VRNotificationService) {
        var viewId;

        var accountDirectiveAPI;
        var accountDirectiveReadyDeferred = UtilsService.createPromiseDeferred();



        var accountTypeAPI;
        var accountTypeReadyDeferred = UtilsService.createPromiseDeferred();

        var gridAPI;
        loadParameters();
        defineScope();
        load();


        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                viewId = parameters.viewId;
            }
        }

        function defineScope() {
            $scope.scopeModel = {};
            var date = new Date();

            $scope.scopeModel.fromDate = new Date(date.getFullYear(), date.getMonth() - 1, 1, 0, 0, 0, 0);

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

            $scope.scopeModel.searchClicked = function () {
                return gridAPI.loadGrid(getFilterObject());
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
                    selectfirstitem:true
                }
                VRUIUtilsService.callDirectiveLoad(accountTypeAPI, payLoad, loadAccountTypeSelectorPromiseDeferred);
            });
            return loadAccountTypeSelectorPromiseDeferred.promise.then(function () {
                $scope.scopeModel.isLoading = false;
                $scope.scopeModel.hideAccountType = accountTypeAPI.hasSingleItem();
            });
        }

       
        
        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadAccountDirective])
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

        function getFilterObject() {
            var accountObject;
            if (accountDirectiveAPI != undefined)
                accountObject = accountDirectiveAPI.getData();
            var query = {
                FromDate: $scope.scopeModel.fromDate,
                AccountId: accountObject != undefined ? accountObject.selectedIds : undefined,
                AccountTypeId: accountTypeAPI.getSelectedIds()
            };
            return query;
        }
    }

    appControllers.controller('VR_AccountBalance_AccountStatementManagementController', accountStatementManagementController);
})(appControllers);