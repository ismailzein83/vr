(function (appControllers) {

    "use strict";

    cityManagementController.$inject = ['$scope', 'VRCommon_CityService'  ];

    function cityManagementController($scope, VRCommon_CityService) {
        var gridAPI;
        defineScope();
        load();
        var filter = {};

        function defineScope() {
            $scope.searchClicked = function () {
                setFilterObject()
                return gridAPI.loadGrid(filter);
            };

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