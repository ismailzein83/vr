(function (appControllers) {

    "use strict";

    cityManagementController.$inject = ['$scope', 'VRCommon_CityService', 'UtilsService', 'VRUIUtilsService'];

    function cityManagementController($scope, VRCommon_CityService, UtilsService, VRUIUtilsService) {
        var gridAPI;
        var countryDirectiveApi;
        var countryReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        defineScope();
        load();
        var filter = {};

        function defineScope() {
            $scope.searchClicked = function () {
                setFilterObject()
                return gridAPI.loadGrid(filter);
            };

            $scope.onCountryDirectiveReady = function (api) {
                countryDirectiveApi = api;
                countryReadyPromiseDeferred.resolve();
            }

            $scope.onGridReady = function (api) {
                gridAPI = api;            
                api.loadGrid(filter);
            }
            $scope.addNewCity = addNewCity;
        }

        function load() {
            $scope.isGettingData = true;           
            loadAllControls();
           

        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadCountrySelector])
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
        function setFilterObject() {
            filter = {
                Name: $scope.name,
                CountryIds: countryDirectiveApi.getSelectedIds()
            };
           
        }

        function addNewCity() {
            var onCityAdded = function (cityObj) {
                if (gridAPI != undefined) {
                    gridAPI.onCityAdded(cityObj);
                }
                   

            };
            VRCommon_CityService.addCity(onCityAdded);
        }

    }

    appControllers.controller('VRCommon_CityManagementController', cityManagementController); 
})(appControllers);