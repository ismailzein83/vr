(function (appControllers) {

    "use strict";

    countryManagementController.$inject = ['$scope', 'VRCommon_CountryService', 'VRCommon_CountryAPIService'];

    function countryManagementController($scope, VRCommon_CountryService, VRCommon_CountryAPIService) {
        var gridAPI;

        defineScope();
        load();

        var filter = {};

        function defineScope() {
            $scope.searchClicked = function () {
                setFilterObject();
                return gridAPI.loadGrid(filter);
            };

            $scope.onGridReady = function (api) {
                gridAPI = api;
                api.loadGrid(filter);
            };

            $scope.addNewCountry = addNewCountry;
            $scope.hasAddCountryPermission = function () {
                return VRCommon_CountryAPIService.HasAddCountryPermission();
            };


            $scope.uploadNewCountries = function () {

                VRCommon_CountryService.uploadCountrires();
            };
            $scope.hasUploadCountryPermission = function () {
                return VRCommon_CountryAPIService.HasUploadCountryPermission();
            };
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