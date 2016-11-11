(function (appControllers) {

    "use strict";

    supplierPricelistController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRUIUtilsService','VRValidationService'];

    function supplierPricelistController($scope, UtilsService, VRNotificationService, VRUIUtilsService, VRValidationService) {

        var supplierDirectiveApi;
        var supplierReadyPromiseDeferred = UtilsService.createPromiseDeferred();
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
            return UtilsService.waitMultipleAsyncOperations([loadSupplierSelector])
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
        function setFilterObject() {
            filter = {
                SupplierIds: supplierDirectiveApi.getSelectedIds(),
                FromDate: $scope.fromDate,
                ToDate: $scope.toDate
            };
        }
    }

    appControllers.controller('WhS_BE_SupplierPricelistController', supplierPricelistController);
})(appControllers);