(function (appControllers) {

    "use strict";

    supplierPricelistController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRUIUtilsService', 'VRValidationService'];

    function supplierPricelistController($scope, UtilsService, VRNotificationService, VRUIUtilsService, VRValidationService) {

        var supplierDirectiveApi;
        var supplierReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var userSelectorApi;
        var userSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var gridAPI;
        var filter = {};
        defineScope();
        load();

        function defineScope() {
            var date = new Date();
            $scope.fromDate = new Date(date.getFullYear(), date.getMonth(), date.getDate(), 0, 0, 0, 0);
            $scope.validateDateTime = function () {
                return VRValidationService.validateTimeRange($scope.fromDate, $scope.toDate);
            };
            $scope.searchClicked = function () {

                if (gridAPI != undefined) {
                    setFilterObject();
                    return gridAPI.loadGrid(filter);
                }
            };

            $scope.onSupplierReady = function (api) {
                supplierDirectiveApi = api;
                supplierReadyPromiseDeferred.resolve();
            };

            $scope.onUserSelectorReady = function (api) {
                userSelectorApi = api;
                userSelectorReadyDeferred.resolve();
            };

            $scope.onGridReady = function (api) {
                gridAPI = api;
                setFilterObject();
                api.loadGrid(filter);
            };


        }

        function load() {
            $scope.isGettingData = true;
            loadAllControls();
        }
        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadSupplierSelector, loadUserSelector])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.isGettingData = false;
              });
        }
        function loadSupplierSelector() {
            var supplierLoadPromiseDeferred = UtilsService.createPromiseDeferred();

            supplierReadyPromiseDeferred.promise
                .then(function () {
                    var directivePayload;
                    VRUIUtilsService.callDirectiveLoad(supplierDirectiveApi, directivePayload, supplierLoadPromiseDeferred);
                });
            return supplierLoadPromiseDeferred.promise;
        }

        function loadUserSelector() {
            var userSelectorLoadDeferred = UtilsService.createPromiseDeferred();
            userSelectorReadyDeferred.promise.then(function () {
                var userSelectorPayload = {
            };
                VRUIUtilsService.callDirectiveLoad(userSelectorApi, userSelectorPayload, userSelectorLoadDeferred);
            });
            return userSelectorLoadDeferred.promise;
        }

        function setFilterObject() {
            filter = {
                SupplierIds: supplierDirectiveApi.getSelectedIds(),
                FromDate: $scope.fromDate,
                ToDate: $scope.toDate,
                UserIds: userSelectorApi.getSelectedIds()
            };
        }
    }

    appControllers.controller('WhS_BE_SupplierPricelistController', supplierPricelistController);
})(appControllers);