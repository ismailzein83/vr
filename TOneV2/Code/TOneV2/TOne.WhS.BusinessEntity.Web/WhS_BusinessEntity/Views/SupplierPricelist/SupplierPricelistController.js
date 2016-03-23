(function (appControllers) {

    "use strict";

    supplierPricelistController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRUIUtilsService'];

    function supplierPricelistController($scope, UtilsService, VRNotificationService, VRUIUtilsService) {

        var supplierDirectiveApi;
        var supplierReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        var gridAPI;
        var filter = {};
        defineScope();
        load();

        function defineScope() {
            $scope.searchClicked = function () {

                if (gridAPI != undefined) {
                    setFilterObject();
                    return gridAPI.loadGrid(filter);
                }

            };
            $scope.onSupplierReady = function (api) {
                supplierDirectiveApi = api;
                supplierReadyPromiseDeferred.resolve();
            }
            $scope.onGridReady = function (api) {
                gridAPI = api;
                api.loadGrid(filter);
            }
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
                    var directivePayload = {};
                    VRUIUtilsService.callDirectiveLoad(supplierDirectiveApi, directivePayload, supplierLoadPromiseDeferred);
                });
            return supplierLoadPromiseDeferred.promise;
        }
        function setFilterObject() {
            filter = {
                SupplierId: supplierDirectiveApi.getSelectedIds()
            };
        }
    }

    appControllers.controller('WhS_BE_SupplierPricelistController', supplierPricelistController);
})(appControllers);