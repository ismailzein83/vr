(function (appControllers) {

    "use strict";

    supplierRateManagementController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRUIUtilsService', 'VRDateTimeService'];

    function supplierRateManagementController($scope, utilsService, vrNotificationService, vruiUtilsService, VRDateTimeService) {
        var gridAPI;
        var supplierDirectiveApi;
        var supplierReadyPromiseDeferred = utilsService.createPromiseDeferred();

        var countryDirectiveApi;
        var countryReadyPromiseDeferred = utilsService.createPromiseDeferred();

        defineScope();
        load();
        var payload = {};

        function defineScope() {
            $scope.effectiveOn = VRDateTimeService.getNowDateTime();

            $scope.searchClicked = function () {
                setFilterObject();
                return gridAPI.loadGrid(payload);
            };


            $scope.resetDate = function () {
                if ($scope.IsPending)
                    $scope.effectiveOn = utilsService.getDateFromDateTime(VRDateTimeService.getNowDateTime());
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
            return utilsService.waitMultipleAsyncOperations([loadSupplierSelector, loadCountrySelector])
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

        function loadCountrySelector() {
            var countryLoadPromiseDeferred = utilsService.createPromiseDeferred();
            countryReadyPromiseDeferred.promise
                .then(function () {
                    var directivePayload = {};
                    vruiUtilsService.callDirectiveLoad(countryDirectiveApi, directivePayload, countryLoadPromiseDeferred);
                });
            return countryLoadPromiseDeferred.promise;
        }

        function setFilterObject() {
            payload = {
                $type: "TOne.WhS.BusinessEntity.Business.SupplierRateQueryHandler,TOne.WhS.BusinessEntity.Business",
                IsSystemCurrency: $scope.isSystemCurrency,
                EffectiveOn: $scope.effectiveOn,
                Query: {
                    SupplierId: supplierDirectiveApi.getSelectedIds(),
                    CountriesIds: countryDirectiveApi.getSelectedIds(),
                    SupplierZoneName: $scope.supplierZoneName,
                    ShowPending: $scope.IsPending
                }
            };
        }
    }

    appControllers.controller('WhS_BE_SupplierRateManagementController', supplierRateManagementController);
})(appControllers);