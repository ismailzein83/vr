(function (appControllers) {

    "use strict";

    Qm_CliTester_HistoryTestCallManagementController.$inject = ['$scope', 'Qm_CliTester_TestCallAPIService', 'UtilsService', 'Qm_CliTester_CallTestResultEnum',
        'Qm_CliTester_CallTestStatusEnum', 'UsersAPIService'];

    function Qm_CliTester_HistoryTestCallManagementController($scope, Qm_CliTester_TestCallAPIService, UtilsService, Qm_CliTester_CallTestResultEnum,
        Qm_CliTester_CallTestStatusEnum, UsersAPIService) {
        var gridAPI;
        var filter = {};

        defineScope();
        load();

        function defineScope() {

            $scope.countries = [];
            $scope.breakouts = [];
            $scope.suppliers = [];
            $scope.testStatus = [];
            $scope.testResult = [];
            $scope.users = [];

            $scope.selectedSupplier;
            $scope.selectedCountry;
            $scope.selectedBreakout;

            $scope.selectedtestResults = [];
            $scope.selectedtestStatus = [];
            $scope.selectedUsers = [];
            for (var prop in Qm_CliTester_CallTestStatusEnum) {
                $scope.testStatus.push(Qm_CliTester_CallTestStatusEnum[prop]);
            }

            for (var prop in Qm_CliTester_CallTestResultEnum) {
                $scope.testResult.push(Qm_CliTester_CallTestResultEnum[prop]);
            }

       

            $scope.searchClicked = function () {
                if (gridAPI != undefined) {
                    setFilterObject();
                    return gridAPI.loadGrid(filter);
                }
            };

            $scope.onGridReady = function (api) {
                gridAPI = api;
                //api.loadGrid(filter);
            }
        }

        function load() {
            $scope.isGettingData = true;
            loadUsers();

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
                $scope.suppliers.length = 0;
                angular.forEach(response, function (itm) {
                    $scope.suppliers.push(itm);
                });
            });
        }

        function loadUsers() {
            return UsersAPIService.GetUsers().then(function (response) {
                $scope.users = response;
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

            if ($scope.fromDate == undefined)
                filter.FromTime = null;
            else {
                filter.FromTime = $scope.fromDate;
            }

            if ($scope.toDate == undefined)
                filter.ToTime = null;
            else {
                filter.ToTime = $scope.toDate;
            }

            if ($scope.selectedtestStatus == undefined)
                filter.CallTestStatus = null;
            else {
                filter.CallTestStatus = UtilsService.getPropValuesFromArray($scope.selectedtestStatus,"value");
            }

            if ($scope.selectedtestResults == undefined)
                filter.CallTestResult = null;
            else {
                filter.CallTestResult = UtilsService.getPropValuesFromArray($scope.selectedtestResults, "value");
            }

            if ($scope.selectedUsers == undefined)
                filter.UserIds = null;
            else {
                filter.UserIds = $scope.selectedUsers;
            }
        }
    }

    appControllers.controller('Qm_CliTester_HistoryTestCallManagementController', Qm_CliTester_HistoryTestCallManagementController);
})(appControllers);