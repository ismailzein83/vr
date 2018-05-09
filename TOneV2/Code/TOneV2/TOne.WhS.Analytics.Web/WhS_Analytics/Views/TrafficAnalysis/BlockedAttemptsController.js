"use strict";
BlockedAttemptsController.$inject = ["$scope", "UtilsService", "VRNavigationService", "WhS_BE_SaleZoneAPIService", "VRNotificationService", "VRUIUtilsService", "VRValidationService", "PeriodEnum"];

function BlockedAttemptsController($scope, UtilsService, VRNavigationService, WhS_BE_SaleZoneAPIService, VRNotificationService, VRUIUtilsService, VRValidationService, PeriodEnum) {

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

        $scope.onTimeRangeDirectiveReady = function (api) {
            timeRangeDirectiveAPI = api;
            timeRangeReadyPromiseDeferred.resolve();
        };

        $scope.onCustomerAccountDirectiveReady = function (api) {
            customerAccountDirectiveAPI = api;
            customerAccountReadyPromiseDeferred.resolve();
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
        return UtilsService.waitMultipleAsyncOperations([loadCustomers, loadSaleZoneSection, loadSwitches, loadTimeRangeSelector])
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
        filter.CustomerIds = customerAccountDirectiveAPI.getSelectedIds();
        filter.SaleZoneIds = saleZoneDirectiveAPI.getSelectedIds();
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

    function loadSaleZoneSection() {
        var loadSaleZonePromiseDeferred = UtilsService.createPromiseDeferred();
        saleZoneReadyPromiseDeferred.promise.then(function () {
            VRUIUtilsService.callDirectiveLoad(saleZoneDirectiveAPI, undefined, loadSaleZonePromiseDeferred);
        });
        return loadSaleZonePromiseDeferred.promise;
    }

    function loadCustomers() {
        var loadCustomerAccountPromiseDeferred = UtilsService.createPromiseDeferred();
        customerAccountReadyPromiseDeferred.promise.then(function () {
            VRUIUtilsService.callDirectiveLoad(customerAccountDirectiveAPI, undefined, loadCustomerAccountPromiseDeferred);
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

appControllers.controller("WhS_Analytics_BlockedAttemptsController", BlockedAttemptsController);