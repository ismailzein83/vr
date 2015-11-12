(function (appControllers) {

    "use strict";

    cityManagementController.$inject = ['$scope', 'VRCommon_CityService'  ];

    function cityManagementController($scope, VRCommon_CityService) {
        var gridAPI;
        var countryDirectiveApi;
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
                api.load();
            }

            $scope.onGridReady = function (api) {
                gridAPI = api;            
                api.loadGrid(filter);
            }
            $scope.addNewCity = addNewCity;
        }

        function load() {

           

        }

        function setFilterObject() {
            filter = {
                Name: $scope.name,
                CountryIds: countryDirectiveApi.getIdsData()
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