(function (appControllers) {

    "use strict";

    countryManagementController.$inject = ['$scope', 'WhS_BE_MainService'  ];

    function countryManagementController($scope, WhS_BE_MainService ) {
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
            WhS_BE_MainService.addCountry(onCountryAdded);
        }

    }

    appControllers.controller('WhS_BE_CountryManagementController', countryManagementController);
})(appControllers);