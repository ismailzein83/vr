"use strict";
RepeatedNumberController.$inject = ['$scope', 'UtilsService', 'VRNavigationService', 'VRNotificationService', 'VRUIUtilsService', 'VRValidationService', 'PeriodEnum'];

function RepeatedNumberController($scope, UtilsService, VRNavigationService, VRNotificationService, VRUIUtilsService, VRValidationService, PeriodEnum) {

    var receivedSwitchIds;
    var mainGridAPI;

    var switchDirectiveAPI;
    var switchReadyPromiseDeferred = UtilsService.createPromiseDeferred();
    
    var customerApi;
    var customerReadyPromiseDeferred = UtilsService.createPromiseDeferred();

    var supplierApi;
    var supplierReadyPromiseDeferred = UtilsService.createPromiseDeferred();

    var timeRangeDirectiveAPI;
    var timeRangeReadyPromiseDeferred = UtilsService.createPromiseDeferred();

    var callStatusDirectiveAPI;
    var callStatusReadyPromiseDeferred = UtilsService.createPromiseDeferred();

    var phoneNumberDirectiveAPI;
    var phoneNumberReadyPromiseDeferred = UtilsService.createPromiseDeferred();

    defineScope();
    load();

    function defineScope() {
        $scope.validateDateTime = function () {
            return VRValidationService.validateTimeRange($scope.fromDate, $scope.toDate);
        };

        $scope.onSwitchDirectiveReady = function (api) {
            switchDirectiveAPI = api;
            switchReadyPromiseDeferred.resolve();
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


        $scope.search = function () {
            return mainGridAPI.loadGrid(getQuery());
        };
    }

    function getQuery() {
        var filter = buildFilter();

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
        return UtilsService.waitMultipleAsyncOperations([loadSwitches, loadCustomerSelector, loadSupplierSelector, loadPhoneNumber, loadTimeRangeSelector, loadCallStatus])
            .catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            })
            .finally(function () {
                $scope.isLoading = false;
            });
    }

    function buildFilter() {
        var filter = {};
        filter.SwitchIds = switchDirectiveAPI.getSelectedIds();
        filter.CustomerIds = customerApi.getSelectedIds();
        filter.SupplierIds = supplierApi.getSelectedIds();

        return filter;
    }

    function loadSwitches() {
        var loadSwitchPromiseDeferred = UtilsService.createPromiseDeferred();
        switchReadyPromiseDeferred.promise.then(function () {
            var payload = {
                selectedIds: receivedSwitchIds
            };
            VRUIUtilsService.callDirectiveLoad(switchDirectiveAPI, payload, loadSwitchPromiseDeferred);
        });
        return loadSwitchPromiseDeferred.promise;
    }

    function loadCustomerSelector() {
        var loadCustomerPromiseDeferred = UtilsService.createPromiseDeferred();
        customerReadyPromiseDeferred.promise.then(function () {
            VRUIUtilsService.callDirectiveLoad(customerApi, undefined, loadCustomerPromiseDeferred);
        });
        return loadCustomerPromiseDeferred.promise;
    }

    function loadSupplierSelector() {
        var loadSupplierPromiseDeferred = UtilsService.createPromiseDeferred();
        supplierReadyPromiseDeferred.promise.then(function () {
            VRUIUtilsService.callDirectiveLoad(supplierApi, undefined, loadSupplierPromiseDeferred);
        });
        return loadSupplierPromiseDeferred.promise;
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

appControllers.controller('WhS_Analytics_RepeatedNumberController', RepeatedNumberController);