(function (app) {

    "use strict";

    accountBalancesManagementController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService', 'VRNavigationService', 'VR_AccountBalance_AccountTypeAPIService', 'VR_AccountBalance_BillingTransactionService'];

    function accountBalancesManagementController($scope, UtilsService, VRUIUtilsService, VRNotificationService, VRNavigationService, VR_AccountBalance_AccountTypeAPIService, VR_AccountBalance_BillingTransactionService) {
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
            $scope.onGridReady = function (api) {
                gridAPI = api;
            };
            $scope.searchClicked = function (api) {
                var payload = {
                    query: getFilterObject()
                }
                gridAPI.loadGrid(payload);
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
            };
        }
    }

    app.controller('VR_AccountBalance_AccountBalancesManagementController', accountBalancesManagementController);
})(app);