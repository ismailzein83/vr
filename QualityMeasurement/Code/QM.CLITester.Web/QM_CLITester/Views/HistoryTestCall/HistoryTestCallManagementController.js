(function (appControllers) {

    "use strict";

    Qm_CliTester_HistoryTestCallManagementController.$inject = ['$scope', 'UtilsService', 'Qm_CliTester_CallTestResultEnum', 'Qm_CliTester_CallTestStatusEnum',
        'VR_Sec_UserAPIService', 'VRUIUtilsService'];

    function Qm_CliTester_HistoryTestCallManagementController($scope, UtilsService, Qm_CliTester_CallTestResultEnum, Qm_CliTester_CallTestStatusEnum,
        VR_Sec_UserAPIService, VRUIUtilsService) {

        var gridAPI;

        var profileDirectiveAPI;
        var profileReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var scheduleDirectiveAPI;
        var scheduleReadyPromiseDeferred = UtilsService.createPromiseDeferred();

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
            $scope.schedules = [];
            $scope.testStatus = [];
            $scope.testResult = [];
            $scope.users = [];
            $scope.fromDate = new Date();
            $scope.fromDate.setHours(0, 0, 0, 0);

            $scope.selectedSuppliers = [];
            $scope.selectedProfiles = [];
            $scope.selectedSchedules = [];
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

            $scope.onScheduleDirectiveReady = function (api) {
                scheduleDirectiveAPI = api;
                scheduleReadyPromiseDeferred.resolve();
            }

            $scope.onSupplierDirectiveReady = function (api) {
                supplierDirectiveAPI = api;
                supplierReadyPromiseDeferred.resolve();
            }

            $scope.onZoneDirectiveReady = function (api) {
                zoneDirectiveAPI = api;
                zoneReadyPromiseDeferred.resolve();
            }

            $scope.onCountrySelectionChanged = function () {
                var countries = countryDirectiveAPI.getSelectedIds();
                var setLoader = function (value) { $scope.isLoadingZoneSelector = value };
                var payload;
                if (countries != undefined && countries.length > 0)
                    payload = {
                        filter: {
                            CountryId: countries
                        }
                    }

                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, zoneDirectiveAPI, payload, setLoader);
            }

        }

        function load() {
            $scope.isGettingData = true;
            loadUsers();

            UtilsService.waitMultipleAsyncOperations([getCountriesInfo, getSuppliersInfo, getProfilesInfo, getSchedulesInfo]).then(function () {
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

        function getSchedulesInfo() {
            var scheduleLoadPromiseDeferred = UtilsService.createPromiseDeferred();

            scheduleReadyPromiseDeferred.promise
                .then(function () {
                    var directivePayload = {
                        isMySchedule: true
                    }

                    VRUIUtilsService.callDirectiveLoad(scheduleDirectiveAPI, directivePayload, scheduleLoadPromiseDeferred);
                });
            return scheduleLoadPromiseDeferred.promise;
        }

        function loadUsers() {
            return VR_Sec_UserAPIService.GetUsers().then(function (response) {
                $scope.users = response;
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
                filter.CountryIds = null;
            else {
                filter.CountryIds = UtilsService.getPropValuesFromArray($scope.selectedCountries, "CountryId");
            }

            if ($scope.selectedProfiles.length == 0 || $scope.selectedProfiles == undefined)
                filter.ProfileIds = null;
            else {
                filter.ProfileIds = [$scope.selectedProfiles.ProfileId];
            }

            if ($scope.selectedScheduleTasks.length == 0)
                filter.ScheduleIDs = null;
            else {
                filter.ScheduleIDs = UtilsService.getPropValuesFromArray($scope.selectedScheduleTasks, "TaskId");
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