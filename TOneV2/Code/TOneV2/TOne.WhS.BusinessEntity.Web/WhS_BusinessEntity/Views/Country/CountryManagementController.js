(function (appControllers) {

    "use strict";

    countryManagementController.$inject = ['$scope', 'WhS_BE_MainService', 'UtilsService', 'VRNotificationService'];

    function countryManagementController($scope, WhS_BE_MainService, UtilsService, VRNotificationService) {
        var gridAPI;
        var carrierProfileDirectiveAPI;
        defineScope();
        load();

        function defineScope() {
            $scope.searchClicked = function () {
                if (!$scope.isGettingData && gridAPI != undefined) {
                   
                    return gridAPI.loadGrid(getFilterObject());
                }
                    
            };

            $scope.onGridReady = function (api) {
                gridAPI = api;            
                api.loadGrid({});
            }
            $scope.AddNewCountry = AddNewCountry;
        }

        function load() {

           

        }

        function getFilterObject() {
            var data = {
                Name: $scope.name,
            };
            return data;
        }

        function AddNewCountry() {
            var onCountryAdded = function (countryObj) {
                if (gridAPI != undefined) {
                    gridAPI.loadGrid({});
                    gridAPI.onCountryAdded(countryObj);

                }
                   

            };
            WhS_BE_MainService.addCountry(onCountryAdded);
        }

    }

    appControllers.controller('WhS_BE_CountryManagementController', countryManagementController);
})(appControllers);