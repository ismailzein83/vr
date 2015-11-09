(function (appControllers) {

    "use strict";

    countryManagementController.$inject = ['$scope', 'VRCommon_CountryService'  ];

    function countryManagementController($scope, VRCommon_CountryService) {
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
            $scope.addNewCountry = addNewCountry;
        }

        function load() {

           

        }

        function setFilterObject() {
            filter = {
                Name: $scope.name,
            };
           
        }

        function addNewCountry() {
            var onCountryAdded = function (countryObj) {
                if (gridAPI != undefined) {
                    gridAPI.onCountryAdded(countryObj);
                }
                   

            };
            VRCommon_CountryService.addCountry(onCountryAdded);
        }

    }

    appControllers.controller('VRCommon_CountryManagementController', countryManagementController); 
})(appControllers);