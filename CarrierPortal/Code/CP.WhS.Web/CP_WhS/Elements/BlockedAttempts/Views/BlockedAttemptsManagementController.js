"use strict";
BlockedAttemptsController.$inject = ["$scope", "UtilsService", "VRNavigationService", "VRNotificationService", "VRUIUtilsService", "VRValidationService", "UISettingsService","PeriodEnum"];

function BlockedAttemptsController($scope, UtilsService, VRNavigationService, VRNotificationService, VRUIUtilsService, VRValidationService, UISettingsService, PeriodEnum) {

    var mainGridAPI;

    var customerAccountDirectiveAPI;
    var customerAccountReadyPromiseDeferred = UtilsService.createPromiseDeferred();

    var saleZoneDirectiveAPI;
    var saleZoneReadyPromiseDeferred = UtilsService.createPromiseDeferred();

    var switchDirectiveAPI;
    var switchReadyPromiseDeferred = UtilsService.createPromiseDeferred();

    var timeRangeDirectiveAPI;
    var timeRangeReadyPromiseDeferred = UtilsService.createPromiseDeferred();

    defineScope();
    load();

    function defineScope() {
        $scope.groupbyNumber = false;
        $scope.fromDate;
        $scope.toDate;

        $scope.selectedPeriod = PeriodEnum.Today;
        var maxSearchRecordCount = UISettingsService.getMaxSearchRecordCount();
        $scope.limit = maxSearchRecordCount;

        $scope.onTimeRangeDirectiveReady = function (api) {
            timeRangeDirectiveAPI = api;
            timeRangeReadyPromiseDeferred.resolve();
        };

        $scope.onCustomerAccountDirectiveReady = function (api) {
            customerAccountDirectiveAPI = api;
            customerAccountReadyPromiseDeferred.resolve();
        };

        $scope.onAccountsReady = function (api) {
            carrierAccountsAPI = api;
            carrierAccountsReadyPromiseDeferred.resolve();
        };

        $scope.onSwitchDirectiveReady = function (api) {
            switchDirectiveAPI = api;
            switchReadyPromiseDeferred.resolve();
        };

        $scope.onSaleZoneDirectiveReady = function (api) {
            saleZoneDirectiveAPI = api;
            saleZoneReadyPromiseDeferred.resolve();
        };

        $scope.onMainGridReady = function (api) {
            mainGridAPI = api;
        };

        $scope.getData = function () {
            return mainGridAPI.loadGrid(getQuery());
        };
        $scope.checkMaxNumberResords = function () {
            if ($scope.limit <= maxSearchRecordCount && maxSearchRecordCount != undefined) {
                return null;
            }
            else {
                return "Max number can be entered is: " + maxSearchRecordCount;
            }
        };

    }

    function getQuery() {
        var filter = buildFilter();
        var query = {
            Filter: filter,
            From: $scope.fromDate,
            To: $scope.toDate,
            Period: $scope.selectedPeriod.value
        };
        return query;
    }

    function load() {
        $scope.isLoading = true;
        loadAllControls();
    }

    function loadAllControls() {
        return UtilsService.waitMultipleAsyncOperations([loadTimeRangeSelector, loadCustomers])
            .catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            })
            .finally(function () {
                $scope.isLoading = false;
            });
    }

    function buildFilter() {
        var filter = {};
        filter.CustomerIds = customerAccountDirectiveAPI.getSelectedIds();
        filter.GroupByNumber = $scope.groupbyNumber;
        return filter;
    }

    function loadSwitches() {
        var loadSwitchPromiseDeferred = UtilsService.createPromiseDeferred();
        switchReadyPromiseDeferred.promise.then(function () {
            VRUIUtilsService.callDirectiveLoad(switchDirectiveAPI, undefined, loadSwitchPromiseDeferred);
        });
        return loadSwitchPromiseDeferred.promise;
    }

    function loadCustomers() {
        var loadCustomerAccountPromiseDeferred = UtilsService.createPromiseDeferred();
        customerAccountReadyPromiseDeferred.promise.then(function () {
            var payload = {
               // businessEntityDefinitionId:"32cc6b0b-785c-437c-9a58-bab8753e50ee"
            };
            VRUIUtilsService.callDirectiveLoad(customerAccountDirectiveAPI, payload, loadCustomerAccountPromiseDeferred);
        });

        return loadCustomerAccountPromiseDeferred.promise;
    }

    function loadTimeRangeSelector() {
        var timeRangeLoadPromiseDeferred = UtilsService.createPromiseDeferred();
        timeRangeReadyPromiseDeferred.promise.then(function () {
            VRUIUtilsService.callDirectiveLoad(timeRangeDirectiveAPI, undefined, timeRangeLoadPromiseDeferred);
        });
        return timeRangeLoadPromiseDeferred.promise;
    }



};

appControllers.controller("CP_WhS_BlockedAttemptsManagementController", BlockedAttemptsController);