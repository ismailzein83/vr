(function (appControllers) {

    "use strict";

    supplierRateManagementController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRUIUtilsService'];

    function supplierRateManagementController($scope, utilsService, vrNotificationService, vruiUtilsService) {
        var gridAPI;
        var supplierDirectiveApi;
        var supplierReadyPromiseDeferred = utilsService.createPromiseDeferred();
        var supplierZoneDirectiveAPI;
        var supplierZoneReadyPromiseDeferred = utilsService.createPromiseDeferred();
        defineScope();
        load();
        var payload = {};

        function defineScope() {
            $scope.effectiveOn = utilsService.getDateFromDateTime(new Date());

            $scope.searchClicked = function () {
                setFilterObject();
                return gridAPI.loadGrid(payload);
            };

            $scope.onSupplierZoneDirectiveReady = function (api) {
                supplierZoneDirectiveAPI = api;
                supplierZoneReadyPromiseDeferred.resolve();
            };

            $scope.resetDate = function () {
                if ($scope.IsPending)
                    $scope.effectiveOn = utilsService.getDateFromDateTime(new Date());
            };
            $scope.onSelectSupplier = function (selectedItem) {
                $scope.showSupplierZoneSelector = true;
                $scope.selectedSupplierZones.length = 0;

                var payload = {
                    supplierId: selectedItem.CarrierAccountId
                };

                var setLoader = function (value) { $scope.isLoadingSaleZonesSection = value; };
                vruiUtilsService.callDirectiveLoadOrResolvePromise($scope, supplierZoneDirectiveAPI, payload, setLoader);
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
            return utilsService.waitMultipleAsyncOperations([loadSupplierSelector, loadSupplierZoneSection])
               .catch(function (error) {
                   vrNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.isGettingData = false;
              });
        }

        function loadSupplierSelector() {
            var supplierLoadPromiseDeferred = utilsService.createPromiseDeferred();

            supplierReadyPromiseDeferred.promise
                .then(function () {
                    var directivePayload = {};
                    vruiUtilsService.callDirectiveLoad(supplierDirectiveApi, directivePayload, supplierLoadPromiseDeferred);
                });
            return supplierLoadPromiseDeferred.promise;
        }

        function setFilterObject() {
            payload = {
                $type: "TOne.WhS.BusinessEntity.Business.SupplierRateQueryHandler,TOne.WhS.BusinessEntity.Business",
                Query: {
                    SupplierId: supplierDirectiveApi.getSelectedIds(),
                    EffectiveOn: $scope.effectiveOn,
                    ZoneIds: supplierZoneDirectiveAPI.getSelectedIds(),
                    ShowPending: $scope.IsPending
                }
            };
        }
    }

    appControllers.controller('WhS_BE_SupplierRateManagementController', supplierRateManagementController);
})(appControllers);