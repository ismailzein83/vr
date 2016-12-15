(function (appControllers) {

    "use strict";

    accountStatementManagementController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VR_AccountBalance_AccountTypeAPIService','VRNotificationService'];

    function accountStatementManagementController($scope, UtilsService, VRUIUtilsService, VRNavigationService, VR_AccountBalance_AccountTypeAPIService, VRNotificationService) {
        var accountTypeId;
        var accountSelectorAPI;
        var accountSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var gridAPI;
        loadParameters();
        defineScope();
        load();


        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                accountTypeId = parameters.accountTypeId;
            }
        }

        function defineScope() {
            var date = new Date();
            $scope.fromDate = new Date(date.getFullYear(), date.getMonth(), 1, 0, 0, 0, 0);
            $scope.onGridReady = function (api) {
                gridAPI = api;
            };
            $scope.onAccountSelectorReady = function (api) {
                accountSelectorAPI = api;
                accountSelectorReadyDeferred.resolve();
            };
            $scope.searchClicked = function () {
                return gridAPI.loadGrid(getFilterObject());

            };
        }

        function load() {
            $scope.isLoadingFilters = true;
            getAccountSelector().then(function () {
                loadAllControls();
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
                $scope.isLoadingFilters = false;
            });

        }
        function getAccountSelector()
        {
            return VR_AccountBalance_AccountTypeAPIService.GetAccountSelector(accountTypeId).then(function (response) {
                $scope.accountSelector = response;
            });
        }
        function getFilterObject() {
            var accountObject;
            if (accountSelectorAPI != undefined)
                accountObject = accountSelectorAPI.getData();
            console.log(accountObject);
            var query = {
                    FromDate: $scope.fromDate,
                    AccountId: accountObject != undefined ? accountObject.selectedIds : undefined,
                    AccountTypeId: accountTypeId,
            };
            console.log(query);
            return query;
        }
        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadAccountSelectorDirective])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.isLoadingFilters = false;
              });
        }
        function loadAccountSelectorDirective() {
            var accountSelectorPayloadLoadDeferred = UtilsService.createPromiseDeferred();
            accountSelectorReadyDeferred.promise.then(function () {
                var accountSelectorPayload;
                VRUIUtilsService.callDirectiveLoad(accountSelectorAPI, accountSelectorPayload, accountSelectorPayloadLoadDeferred);
            });
            return accountSelectorPayloadLoadDeferred.promise;
        }
    }

    appControllers.controller('VR_AccountBalance_AccountStatementManagementController', accountStatementManagementController);
})(appControllers);