(function (appControllers) {

    "use strict";

    Qm_CliTester_HistoryTestCallManagementController.$inject = ['$scope', 'Qm_CliTester_TestCallAPIService', 'UtilsService', 'Qm_CliTester_CallTestResultEnum', 'Qm_CliTester_CallTestStatusEnum',
        'UsersAPIService', 'VRUIUtilsService'];

    function Qm_CliTester_HistoryTestCallManagementController($scope, Qm_CliTester_TestCallAPIService, UtilsService, Qm_CliTester_CallTestResultEnum, Qm_CliTester_CallTestStatusEnum,
        UsersAPIService, VRUIUtilsService) {

        var gridAPI;

        var supplierDirectiveAPI;
        var supplierReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var zoneDirectiveAPI;
        var zoneReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var filter = {};

        defineScope();
        load();

        function defineScope() {

            $scope.countries = [];
            $scope.zones = [];
            $scope.suppliers = [];
            $scope.testStatus = [];
            $scope.testResult = [];
            $scope.users = [];

            $scope.selectedSuppliers = [];
            $scope.selectedZones = [];

            $scope.selectedCountry;

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
            }


            $scope.onSupplierDirectiveReady = function (api) {
                supplierDirectiveAPI = api;
                supplierReadyPromiseDeferred.resolve();
            }

            $scope.onZoneDirectiveReady = function (api) {
                zoneDirectiveAPI = api;
                zoneReadyPromiseDeferred.resolve();
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
            var supplierLoadPromiseDeferred = UtilsService.createPromiseDeferred();

            supplierReadyPromiseDeferred.promise
                .then(function () {
                    var directivePayload;

                    VRUIUtilsService.callDirectiveLoad(supplierDirectiveAPI, directivePayload, supplierLoadPromiseDeferred);
                });
            return supplierLoadPromiseDeferred.promise;
        }

        function loadUsers() {
            return UsersAPIService.GetUsers().then(function (response) {
                $scope.users = response;
            });
        }

        $scope.previewZones = function () {
            if ($scope.selectedCountry != undefined) {
                getZonesInfo($scope.selectedCountry.Id);
            }
        }

        function getZonesInfo(selectedCountryId) {
            $scope.isLoading = true;
            zoneReadyPromiseDeferred.promise
                .then(function () {
                    var payload = {
                        countryId: selectedCountryId
                    };

                    zoneDirectiveAPI.load(payload);
                }).finally(function () {
                    $scope.isLoading = false;
                });
        }

        function setFilterObject() {

            if ($scope.selectedSuppliers.length == 0)
                filter.SupplierIDs = null;
            else {
                filter.SupplierIDs = UtilsService.getPropValuesFromArray($scope.selectedSuppliers, "SupplierId");
            }


            if ($scope.selectedZones.length == 0)
                filter.ZoneIDs = null;
            else {
                filter.ZoneIDs = UtilsService.getPropValuesFromArray($scope.selectedZones, "ZoneId");
            }


            if ($scope.selectedCountry == undefined)
                filter.CountryID = null;
            else {
                filter.CountryID = $scope.selectedCountry.Id;
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
                filter.CallTestStatus = UtilsService.getPropValuesFromArray($scope.selectedtestStatus, "value");
            }

            if ($scope.selectedtestResults == undefined)
                filter.CallTestResult = null;
            else {
                filter.CallTestResult = UtilsService.getPropValuesFromArray($scope.selectedtestResults, "value");
            }

            if ($scope.selectedUsers == undefined)
                filter.UserIds = null;
            else {
                filter.UserIds = UtilsService.getPropValuesFromArray($scope.selectedUsers, "UserId");
            }
            console.log(filter)
        }
    }

    appControllers.controller('Qm_CliTester_HistoryTestCallManagementController', Qm_CliTester_HistoryTestCallManagementController);
})(appControllers);