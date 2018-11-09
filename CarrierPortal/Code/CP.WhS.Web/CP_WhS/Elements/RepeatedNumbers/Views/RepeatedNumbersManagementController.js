"use strict";
RepeatedNumberController.$inject = ['$scope', 'UtilsService', 'VRNavigationService', 'VRNotificationService', 'VRUIUtilsService', 'VRValidationService', 'PeriodEnum', "CP_WhS_AccountViewTypeEnum"];

function RepeatedNumberController($scope, UtilsService, VRNavigationService, VRNotificationService, VRUIUtilsService, VRValidationService, PeriodEnum, CP_WhS_AccountViewTypeEnum) {

    var mainGridAPI;

    var timeRangeDirectiveAPI;
    var timeRangeReadyPromiseDeferred = UtilsService.createPromiseDeferred();

    var accountDirectiveAPI;
    var accountReadyPromiseDeferred = UtilsService.createPromiseDeferred();


    var callStatusDirectiveAPI;
    var callStatusReadyPromiseDeferred = UtilsService.createPromiseDeferred();

    var phoneNumberDirectiveAPI;
    var phoneNumberReadyPromiseDeferred = UtilsService.createPromiseDeferred();

    var accountViewTypeAPI;
    var accountViewTypeReadyPromiseDeferred = UtilsService.createPromiseDeferred();


    defineScope();
    load();

    function defineScope() {
        $scope.validateDateTime = function () {
            return VRValidationService.validateTimeRange($scope.fromDate, $scope.toDate);
        };

        $scope.onAccountDirectiveReady = function (api) {
            accountDirectiveAPI = api;
            accountReadyPromiseDeferred.resolve();
        };

        $scope.onCustomerDirectiveReady = function (api) {
            customerApi = api;
            customerReadyPromiseDeferred.resolve();
        };

        $scope.onSupplierDirectiveReady = function (api) {
            supplierApi = api;
            supplierReadyPromiseDeferred.resolve();
        };

        $scope.nRecords = '10';
        $scope.fromDate;
        $scope.toDate;

        $scope.selectedPeriod = PeriodEnum.Today;

        $scope.onTimeRangeDirectiveReady = function (api) {
            timeRangeDirectiveAPI = api;
            timeRangeReadyPromiseDeferred.resolve();
        };

        $scope.CDROption = [];
        $scope.PhoneNumber = [];

        $scope.onMainGridReady = function (api) {
            mainGridAPI = api;
        };

        $scope.onCallStatusDirectiveReady = function (api) {
            callStatusDirectiveAPI = api;
            callStatusReadyPromiseDeferred.resolve();
        };

        $scope.onPhoneNumberDirectiveReady = function (api) {
            phoneNumberDirectiveAPI = api;
            phoneNumberReadyPromiseDeferred.resolve();
        };

        $scope.onAccountViewTypeSelectorReady = function (api) {
            accountViewTypeAPI = api;
            accountViewTypeReadyPromiseDeferred.resolve();
        };
        $scope.onAccountViewTypeSelectionChange = function (value) {
            if (value != undefined) {
                var setLoader = function (value) {
                    $scope.isLoading = value;
                };
                var accountViewTypePayload = {
                    businessEntityDefinitionId: accountViewTypeAPI.getSelectedIds() == CP_WhS_AccountViewTypeEnum.Customer.value ? "32cc6b0b-785c-437c-9a58-bab8753e50ee" : "574ef14e-64d3-4f56-8d0a-8ec05f043b7b"
                };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, accountDirectiveAPI, accountViewTypePayload, setLoader, undefined);
            }
            $scope.selectedCarrierAccounts.length = 0;
        };

        $scope.search = function () {
            return mainGridAPI.loadGrid(getQuery());
        };
    }

    function getQuery() {
        var filter = buildFilter();
        $scope.accountViewType = accountViewTypeAPI.getSelectedIds();
        var query = {
            Filter: filter,
            From: $scope.fromDate,
            To: $scope.toDate,
            RepeatedMorethan: $scope.nRecords,
            CDRType: callStatusDirectiveAPI.getSelectedIds(),
            PhoneNumberType: phoneNumberDirectiveAPI.getSelectedValues(),
            Period: $scope.selectedPeriod.value,
            PhoneNumber: $scope.phoneNumber
        };
        return query;
    }

    function load() {
        $scope.isLoading = true;
        loadAllControls();
    }

    function loadAllControls() {
        return UtilsService.waitMultipleAsyncOperations([loadPhoneNumber, loadTimeRangeSelector, loadCallStatus, loadAccountViewTypeSelector])
            .catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            })
            .finally(function () {
                $scope.isLoading = false;
            });
    }

    function buildFilter() {
        var filter = {};
        if (accountViewTypeAPI.getSelectedIds() == CP_WhS_AccountViewTypeEnum.Customer.value) {
            filter.CustomerIds = accountDirectiveAPI.getSelectedIds();
        }
        else {
            filter.SupplierIds = accountDirectiveAPI.getSelectedIds();
        }
        return filter;
    }

    function loadAccountViewTypeSelector() {
        var accountViewTypeLoadPromiseDeferred = UtilsService.createPromiseDeferred();
        accountViewTypeReadyPromiseDeferred.promise.then(function () {
            VRUIUtilsService.callDirectiveLoad(accountViewTypeAPI, undefined, accountViewTypeLoadPromiseDeferred);
        });
        return accountViewTypeLoadPromiseDeferred.promise;
    }


    function loadTimeRangeSelector() {
        var timeRangeLoadPromiseDeferred = UtilsService.createPromiseDeferred();
        timeRangeReadyPromiseDeferred.promise.then(function () {

            VRUIUtilsService.callDirectiveLoad(timeRangeDirectiveAPI, undefined, timeRangeLoadPromiseDeferred);
        });
        return timeRangeLoadPromiseDeferred.promise;
    }

    function loadCallStatus() {
        var callStatusLoadPromiseDeferred = UtilsService.createPromiseDeferred();
        callStatusReadyPromiseDeferred.promise.then(function () {
            var payload = { selectedIds: 0 };
            VRUIUtilsService.callDirectiveLoad(callStatusDirectiveAPI, payload, callStatusLoadPromiseDeferred);
        });
        return callStatusLoadPromiseDeferred.promise;
    }

    function loadPhoneNumber() {
        var phoneNumberLoadPromiseDeferred = UtilsService.createPromiseDeferred();
        phoneNumberReadyPromiseDeferred.promise.then(function () {
            var payload = { selectedIds: 0 };
            VRUIUtilsService.callDirectiveLoad(phoneNumberDirectiveAPI, payload, phoneNumberLoadPromiseDeferred);
        });
        return phoneNumberLoadPromiseDeferred.promise;
    }
};

appControllers.controller('CP_WhS_RepeatedNumbersController', RepeatedNumberController);