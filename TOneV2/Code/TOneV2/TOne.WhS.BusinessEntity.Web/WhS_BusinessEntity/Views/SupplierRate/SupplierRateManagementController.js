(function (appControllers) {

    "use strict";

    supplierRateManagementController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRUIUtilsService'];

    function supplierRateManagementController($scope, utilsService, vrNotificationService, vruiUtilsService) {
        var gridAPI;
        var supplierDirectiveApi;
        var supplierReadyPromiseDeferred = utilsService.createPromiseDeferred();

        var countryDirectiveApi;
        var countryReadyPromiseDeferred = utilsService.createPromiseDeferred();

        defineScope();
        load();
        var payload = {};

        function defineScope() {
            $scope.effectiveOn = new Date();

            $scope.searchClicked = function () {
                setFilterObject();
                return gridAPI.loadGrid(payload);
            };


            $scope.resetDate = function () {
                if ($scope.IsPending)
                    $scope.effectiveOn = utilsService.getDateFromDateTime(new Date());
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
                Query: {
                    SupplierId: supplierDirectiveApi.getSelectedIds(),
                    EffectiveOn: $scope.effectiveOn,
                    CountriesIds: countryDirectiveApi.getSelectedIds(),
                    SupplierZoneName: $scope.supplierZoneName,
                    ShowPending: $scope.IsPending
                }
            };
        }
    }

    appControllers.controller('WhS_BE_SupplierRateManagementController', supplierRateManagementController);
})(appControllers);