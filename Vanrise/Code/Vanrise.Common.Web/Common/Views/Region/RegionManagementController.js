(function (appControllers) {

    "use strict";

    regionManagementController.$inject = ['$scope', 'VRCommon_RegionService','VRCommon_RegionAPIService', 'UtilsService', 'VRUIUtilsService'];

    function regionManagementController($scope, VRCommon_RegionService, VRCommon_RegionAPIService, UtilsService, VRUIUtilsService) {
        var gridAPI;
        var filter = {};

        var countryDirectiveApi;
        var countryReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        defineScope();
        load();

        function defineScope() {
            $scope.searchClicked = function () {
                getFilterObject();
                return gridAPI.loadGrid(filter);
            };
            $scope.hasAddRegionPermission = function () {
                return VRCommon_RegionAPIService.HasAddRegionPermission();
            };
            $scope.onCountryDirectiveReady = function (api) {
                countryDirectiveApi = api;
                countryReadyPromiseDeferred.resolve();
            };

            function getFilterObject() {
                filter = {
                    Name: $scope.name,
                    CountryIds: countryDirectiveApi.getSelectedIds()
                };

            }

            $scope.onGridReady = function (api) {
                gridAPI = api;
                api.loadGrid(filter);
            };
            $scope.addNewRegion = addNewRegion;
        }

        function load() {
            $scope.isLoadingFilters = true;
            loadAllControls();
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadCountrySelector])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.isLoadingFilters = false;
              });
        }
        function loadCountrySelector() {
            var countryLoadPromiseDeferred = UtilsService.createPromiseDeferred();

            countryReadyPromiseDeferred.promise
                .then(function () {
                    VRUIUtilsService.callDirectiveLoad(countryDirectiveApi, undefined, countryLoadPromiseDeferred);
                });
            return countryLoadPromiseDeferred.promise;
        }


        function addNewRegion() {
            var onRegionAdded = function (regionObj) {
                gridAPI.onRegionAdded(regionObj);
            };

            VRCommon_RegionService.addRegion(onRegionAdded);
        }

    }

    appControllers.controller('VRCommon_RegionManagementController', regionManagementController); 
})(appControllers);