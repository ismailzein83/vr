(function (appControllers) {

    "use strict";

    supplierRateManagementController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRUIUtilsService'];

    function supplierRateManagementController($scope, UtilsService, VRNotificationService, VRUIUtilsService) {
        var gridAPI;
        var supplierDirectiveApi;
        var supplierReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        defineScope();
        load();
        var filter = {};

        function defineScope() {
            $scope.searchClicked = function () {
                setFilterObject();
                return gridAPI.loadGrid(filter);
            };
            $scope.onSupplierReady = function (api) {
                supplierDirectiveApi = api;
                supplierReadyPromiseDeferred.resolve();
            }
            
            $scope.onGridReady = function (api) {
                gridAPI = api;            
               
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
                SupplierId: supplierDirectiveApi.getSelectedIds(),
                EffectiveOn: $scope.effectiveOn
            };
           
        }

    }

    appControllers.controller('WhS_BE_SupplierRateManagementController', supplierRateManagementController);
})(appControllers);