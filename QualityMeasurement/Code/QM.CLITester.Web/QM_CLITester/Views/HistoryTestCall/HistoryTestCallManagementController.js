(function (appControllers) {

    "use strict";

    Qm_CliTester_HistoryTestCallManagementController.$inject = ['$scope', 'Qm_CliTester_TestCallAPIService', 'UtilsService'];

    function Qm_CliTester_HistoryTestCallManagementController($scope, Qm_CliTester_TestCallAPIService, UtilsService) {
        var gridAPI;
        var filter = {};

        defineScope();
        load();

        function defineScope() {

            $scope.countries = [];
            $scope.breakouts = [];
            $scope.suppliers = [];

            $scope.selectedSupplier;
            $scope.selectedCountry;
            $scope.selectedBreakout;

            $scope.searchClicked = function () {
                if (gridAPI != undefined) {
                    setFilterObject();
                    console.log(filter);
                    return gridAPI.loadGrid(filter);
                }
            };

            $scope.onGridReady = function (api) {
                gridAPI = api;
                api.loadGrid(filter);
            }
        }

        function load() {
            $scope.isGettingData = true;
            UtilsService.waitMultipleAsyncOperations([getCountriesInfo, getSuppliersInfo]).then(function () {
            }).finally(function () {
                $scope.isGettingData = false;
            });
        }


        function getCountriesInfo() {
            return Qm_CliTester_TestCallAPIService.GetCountries().then(function (response) {
                $scope.countries.length = 0;
                angular.forEach(response, function (itm) {
                    $scope.countries.push(itm);
                });
            });
        }


        function getSuppliersInfo() {
            return Qm_CliTester_TestCallAPIService.GetSuppliers().then(function (response) {
                console.log(response);
                $scope.suppliers.length = 0;
                angular.forEach(response, function (itm) {
                    $scope.suppliers.push(itm);
                });
            });
        }

        $scope.previewBreakouts = function () {
            if ($scope.selectedCountry != undefined) {
                getBreakoutsInfo($scope.selectedCountry.Id);
            }
        }

        function getBreakoutsInfo(selectedCountryId) {
            $scope.isLoading = true;
            return Qm_CliTester_TestCallAPIService.GetBreakouts(selectedCountryId).then(function (response) {

                $scope.breakouts.length = 0;
                angular.forEach(response, function (itm) {
                    $scope.breakouts.push(itm);
                });
            }).finally(function () {
                $scope.isLoading = false;
            });
        }

        function setFilterObject() {
            console.log("sele");
            console.log($scope.selectedSupplier);
            if ($scope.selectedSupplier == undefined)
                filter.SupplierID = null;
            else {
                filter.SupplierID = $scope.selectedSupplier.Id;
            }

            if ($scope.selectedCountry == undefined)
                filter.CountryID = null;
            else {
                filter.CountryID = $scope.selectedCountry.Id;
            }

            if ($scope.selectedBreakout == undefined)
                filter.ZoneID = null;
            else {
                filter.ZoneID = $scope.selectedBreakout.Id;
            }
        }
    }

    appControllers.controller('Qm_CliTester_HistoryTestCallManagementController', Qm_CliTester_HistoryTestCallManagementController);
})(appControllers);