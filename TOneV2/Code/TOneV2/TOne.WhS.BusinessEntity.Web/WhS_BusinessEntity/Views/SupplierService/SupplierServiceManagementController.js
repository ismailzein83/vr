(function (appControllers) {

    "use strict";

    supplierServiceManagementController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRUIUtilsService', 'VRDateTimeService'];

    function supplierServiceManagementController($scope, UtilsService, VRNotificationService, VRUIUtilsService, VRDateTimeService) {
        var gridAPI;
        var supplierDirectiveApi;
        var supplierReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        var supplierZoneDirectiveAPI;
        var supplierZoneReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        var zoneServiceConfigSelectorReadyDeferred = UtilsService.createPromiseDeferred();
        var zoneServiceConfigSelectorAPI;
        defineScope();
        load();
        var filter = {};

        function defineScope() {
            $scope.effectiveOn = VRDateTimeService.getNowDateTime();

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

            $scope.onZoneServiceConfigSelectorReady = function (api) {
                zoneServiceConfigSelectorAPI = api;
                zoneServiceConfigSelectorReadyDeferred.resolve();
               
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
            return UtilsService.waitMultipleAsyncOperations([loadSupplierSelector, loadZoneServiceConfigSelector, loadSupplierZoneSection])
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
        function loadZoneServiceConfigSelector() {
            var zoneServiceConfigSelectorLoadDeferred = UtilsService.createPromiseDeferred();

            zoneServiceConfigSelectorReadyDeferred.promise.then(function () {
                zoneServiceConfigSelectorReadyDeferred = undefined;
                var zoneServiceConfigSelectorPayload = {
                };
                VRUIUtilsService.callDirectiveLoad(zoneServiceConfigSelectorAPI, zoneServiceConfigSelectorPayload, zoneServiceConfigSelectorLoadDeferred);
            });

            return zoneServiceConfigSelectorLoadDeferred.promise;
        }
        function setFilterObject() {
            filter = {
                SupplierId: supplierDirectiveApi.getSelectedIds(),
                EffectiveOn: $scope.effectiveOn,
                ZoneIds: supplierZoneDirectiveAPI.getSelectedIds(),
                ServiceIds: zoneServiceConfigSelectorAPI.getSelectedIds()
            };
        }

    }

    appControllers.controller('WhS_BE_SupplierServiceManagementController', supplierServiceManagementController);
})(appControllers);