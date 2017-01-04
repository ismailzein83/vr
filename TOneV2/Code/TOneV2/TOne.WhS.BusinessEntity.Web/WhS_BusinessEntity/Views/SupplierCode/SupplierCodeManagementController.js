(function (appControllers) {

    "use strict";

    supplierCodeManagementController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRUIUtilsService'];

    function supplierCodeManagementController($scope, UtilsService, VRNotificationService, VRUIUtilsService) {
        var gridAPI;
        var supplierDirectiveApi;
        var supplierReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        var supplierZoneDirectiveAPI;
        var supplierZoneReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        defineScope();
        load();
        var filter = {};

        function defineScope() {
            $scope.effectiveOn = new Date();
            $scope.searchClicked = function () {
                
                setFilterObject();
                return gridAPI.loadGrid(filter);
            };

            $scope.onSupplierZoneDirectiveReady = function (api) {
                supplierZoneDirectiveAPI = api;
                supplierZoneReadyPromiseDeferred.resolve();
            };


            $scope.onSelectSupplier = function (selectedItem) {
                $scope.showSupplierZoneSelector = true;
                $scope.selectedSupplierZones.length = 0;
                var payload = {
                    supplierId: selectedItem.CarrierAccountId
                };

                var setLoader = function (value) { $scope.isLoadingSaleZonesSection = value; };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, supplierZoneDirectiveAPI, payload, setLoader);
            };



            $scope.onSupplierReady = function (api) {
                supplierDirectiveApi = api;
                supplierReadyPromiseDeferred.resolve();
            };
            
            $scope.onGridReady = function (api) {
                gridAPI = api;

            };
        }

        function load() {
            $scope.isGettingData = true;
            loadAllControls();
        }

        function loadSupplierZoneSection() {
            return supplierZoneReadyPromiseDeferred.promise;
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadSupplierSelector, loadSupplierZoneSection])
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
                EffectiveOn: $scope.effectiveOn,
                ZoneIds: supplierZoneDirectiveAPI.getSelectedIds(),
                Code: $scope.code
            };
           
        }

    }

    appControllers.controller('WhS_BE_SupplierCodeManagementController', supplierCodeManagementController);
})(appControllers);