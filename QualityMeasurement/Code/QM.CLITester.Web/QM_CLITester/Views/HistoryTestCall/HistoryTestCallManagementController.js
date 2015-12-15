(function (appControllers) {

    "use strict";

    Qm_CliTester_HistoryTestCallManagementController.$inject = ['$scope', 'Qm_CliTester_TestCallAPIService', 'UtilsService', 'Qm_CliTester_CallTestResultEnum', 'Qm_CliTester_CallTestStatusEnum',
        'UsersAPIService', 'VRUIUtilsService'];

    function Qm_CliTester_HistoryTestCallManagementController($scope, Qm_CliTester_TestCallAPIService, UtilsService, Qm_CliTester_CallTestResultEnum, Qm_CliTester_CallTestStatusEnum,
        UsersAPIService, VRUIUtilsService) {

        var gridAPI;

        var profileDirectiveAPI;
        var profileReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var supplierDirectiveAPI;
        var supplierReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var zoneDirectiveAPI;
        var zoneReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var countryDirectiveAPI;
        var countryReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var filter = {};

        defineScope();
        load();

        function defineScope() {

            $scope.countries = [];
            $scope.zones = [];
            $scope.suppliers = [];
            $scope.profiles = [];
            $scope.testStatus = [];
            $scope.testResult = [];
            $scope.users = [];

            $scope.selectedSuppliers = [];
            $scope.selectedProfiles = [];
            $scope.selectedZones = [];
            $scope.selectedCountries = [];
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

            $scope.onCountryDirectiveReady = function (api) {
                countryDirectiveAPI = api;
                countryReadyPromiseDeferred.resolve();
            }

            $scope.onProfileDirectiveReady = function (api) {
                profileDirectiveAPI = api;
                profileReadyPromiseDeferred.resolve();
            }

            $scope.onSupplierDirectiveReady = function (api) {
                supplierDirectiveAPI = api;
                supplierReadyPromiseDeferred.resolve();
            }

            $scope.onZoneDirectiveReady = function (api) {
                zoneDirectiveAPI = api;
                zoneReadyPromiseDeferred.resolve();
            }


            $scope.onCountrySelectItem = function (selectedItem) {
                if ($scope.selectedCountries.length == 1 && selectedItem != undefined) {
                    var setLoader = function (value) { $scope.isLoadingZonesSelector = value };

                    var payload = {
                        countryId: selectedItem.CountryId
                    }

                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, zoneDirectiveAPI, payload, setLoader);
                }
                else {
                    $scope.selectedZones.length = 0;
                }
            }

        }

        function load() {
            $scope.isGettingData = true;
            loadUsers();

            UtilsService.waitMultipleAsyncOperations([getCountriesInfo, getSuppliersInfo, getProfilesInfo]).then(function () {
            }).finally(function () {
                $scope.isGettingData = false;
            });
        }


        function getCountriesInfo() {
            var countryLoadPromiseDeferred = UtilsService.createPromiseDeferred();

            countryReadyPromiseDeferred.promise
                .then(function () {
                    var directivePayload;

                    VRUIUtilsService.callDirectiveLoad(countryDirectiveAPI, directivePayload, countryLoadPromiseDeferred);
                });
            return countryLoadPromiseDeferred.promise;
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


        function getProfilesInfo() {
            var profileLoadPromiseDeferred = UtilsService.createPromiseDeferred();

            profileReadyPromiseDeferred.promise
                .then(function () {
                    var directivePayload;

                    VRUIUtilsService.callDirectiveLoad(profileDirectiveAPI, directivePayload, profileLoadPromiseDeferred);
                });
            return profileLoadPromiseDeferred.promise;
        }

        function loadUsers() {
            return UsersAPIService.GetUsers().then(function (response) {
                $scope.users = response;
            });
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

            if ($scope.selectedUsers.length == 0)
                filter.UserIDs = null;
            else {
                filter.UserIDs = UtilsService.getPropValuesFromArray($scope.selectedUsers, "UserId");
            }


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

            if ($scope.selectedCountries.length == 0)
                filter.Countries = null;
            else {
                filter.Countries = UtilsService.getPropValuesFromArray($scope.selectedCountries, "CountryId");
            }

            if ($scope.selectedProfiles.length == 0)
                filter.ProfileIDs = null;
            else {
                filter.ProfileIDs = UtilsService.getPropValuesFromArray($scope.selectedProfiles, "ProfileId");
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
        }
    }

    appControllers.controller('Qm_CliTester_HistoryTestCallManagementController', Qm_CliTester_HistoryTestCallManagementController);
})(appControllers);