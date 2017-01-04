(function (appControllers) {

    "use strict";

    supplierZoneManagementController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRUIUtilsService'];

    function supplierZoneManagementController($scope, UtilsService, VRNotificationService, VRUIUtilsService) {
        var gridAPI;
        var supplierDirectiveApi;
        var supplierReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        var countryDirectiveApi;
        var countryReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        defineScope();
        load();
        var filter = {};

        function defineScope() {
            $scope.effectiveOn = new Date();
            $scope.searchClicked = function () {
                setFilterObject();
                return gridAPI.loadGrid(filter);
            };
            $scope.onSupplierReady = function (api) {
                supplierDirectiveApi = api;
                supplierReadyPromiseDeferred.resolve();
            };
            $scope.onCountryReady = function (api) {
                countryDirectiveApi = api;
                countryReadyPromiseDeferred.resolve();
            };
            $scope.onGridReady = function (api) {
                gridAPI = api;

            };
        }

        function load() {
            $scope.isGettingData = true;
            loadAllControls();
        }
        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadCountrySelector, loadSupplierSelector])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.isGettingData = false;
              });
        }
        function loadCountrySelector() {
            var countryLoadPromiseDeferred = UtilsService.createPromiseDeferred();

            countryReadyPromiseDeferred.promise
                .then(function () {
                    var directivePayload = {};
                    VRUIUtilsService.callDirectiveLoad(countryDirectiveApi, directivePayload, countryLoadPromiseDeferred);
                });
            return countryLoadPromiseDeferred.promise;
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
                Name: $scope.name,
                SupplierId: supplierDirectiveApi.getSelectedIds(),
                EffectiveOn: UtilsService.getDateFromDateTime($scope.effectiveOn),
                Countries: countryDirectiveApi.getSelectedIds()
            };
           
        }

    }

    appControllers.controller('WhS_BE_SupplierZoneManagementController', supplierZoneManagementController);
})(appControllers);