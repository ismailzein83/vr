(function (app) {

    "use strict";

    billingTransactionManagementController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService','VRNavigationService', 'VR_AccountBalance_AccountTypeAPIService'];

    function billingTransactionManagementController($scope, UtilsService, VRUIUtilsService, VRNotificationService, VRNavigationService, VR_AccountBalance_AccountTypeAPIService) {
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
            var today = new Date();
            today.setHours(0, 0, 0, 0);
            $scope.fromTime = today;
            $scope.onGridReady = function (api) {
                gridAPI = api;
                api.loadGrid(getFilterObject());
            };

            $scope.onGridReady = function (api) {
                gridAPI = api;
                api.loadGrid(getFilterObject());
            };
            $scope.onFilterDirectiveReady = function (api) {
                filterDirectiveAPI = api;
                filterDirectiveReadyDeferred.resolve()

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
                FromTime: $scope.fromTime,
                ToTime: $scope.toTime,
                AccountTypeId: accountTypeId
            };
        }
    }

    app.controller('VR_AccountBalance_BillingTransactionManagementController', billingTransactionManagementController);
})(app);