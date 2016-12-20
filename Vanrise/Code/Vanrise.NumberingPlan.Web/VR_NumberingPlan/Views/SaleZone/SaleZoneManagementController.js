(function (appControllers) {

    "use strict";

    saleZoneManagementController.$inject = ['$scope' ,'UtilsService', 'VRNotificationService', 'VRUIUtilsService'];

    function saleZoneManagementController($scope,UtilsService, VRNotificationService, VRUIUtilsService) {
        var gridAPI;
        var sellingDirectiveApi;
        var sellingReadyPromiseDeferred = UtilsService.createPromiseDeferred();
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
            $scope.onSellingNumberReady = function (api) {
                sellingDirectiveApi = api;
                sellingReadyPromiseDeferred.resolve();
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
            return UtilsService.waitMultipleAsyncOperations([loadCountrySelector, loadSellingSelector])
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
        function loadSellingSelector() {
            var sellingLoadPromiseDeferred = UtilsService.createPromiseDeferred();

            sellingReadyPromiseDeferred.promise
                .then(function () {
                    var directivePayload = {};
                    VRUIUtilsService.callDirectiveLoad(sellingDirectiveApi, directivePayload, sellingLoadPromiseDeferred);
                });
            return sellingLoadPromiseDeferred.promise;
        }

        function setFilterObject() {
            filter = {
                Name: $scope.name,
                SellingNumberId: sellingDirectiveApi.getSelectedIds(),
                EffectiveOn: $scope.effectiveOn,
                Countries: countryDirectiveApi.getSelectedIds()
            };
           
        }

    }

    appControllers.controller('Vr_NP_SaleZoneManagementController', saleZoneManagementController);
})(appControllers);