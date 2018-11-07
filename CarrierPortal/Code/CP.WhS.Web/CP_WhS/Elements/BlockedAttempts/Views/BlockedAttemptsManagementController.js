"use strict";
BlockedAttemptsController.$inject = ["$scope", "UtilsService", "VRNavigationService", "VRNotificationService", "VRUIUtilsService", "VRValidationService", "UISettingsService","PeriodEnum"];

function BlockedAttemptsController($scope, UtilsService, VRNavigationService, VRNotificationService, VRUIUtilsService, VRValidationService, UISettingsService, PeriodEnum) {

    var mainGridAPI;

    var customerAccountDirectiveAPI;
    var customerAccountReadyPromiseDeferred = UtilsService.createPromiseDeferred();

    var masterSaleZoneDirectiveAPI;
    var masterSaleZoneReadyPromiseDeferred = UtilsService.createPromiseDeferred();

    var timeRangeDirectiveAPI;
    var timeRangeReadyPromiseDeferred = UtilsService.createPromiseDeferred();

    defineScope();
    load();

    function defineScope() {
        $scope.groupbyNumber = false;
        $scope.fromDate;
        $scope.toDate;

        $scope.selectedPeriod = PeriodEnum.Today;
        $scope.limit = 1000;
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

        $scope.onMasterSaleZoneDirectiveReady = function (api) {
            masterSaleZoneDirectiveAPI = api;
            masterSaleZoneReadyPromiseDeferred.resolve();
        };

        $scope.onMainGridReady = function (api) {
            mainGridAPI = api;
        };

        $scope.getData = function () {
            return mainGridAPI.loadGrid(getQuery());
        };
        $scope.checkMaxNumberResords = function () {
            if ($scope.limit <= 1000) {
                return null;
            }
            else {
                return "The maximum number that can be entered is: " + 1000;
            }
        };

    }

    function getQuery() {
        var filter = buildFilter();
        var query = {
            Filter: filter,
            From: $scope.fromDate,
            To: $scope.toDate,
            Top: $scope.limit,
            Period: $scope.selectedPeriod.value
        };
        return query;
    }

    function load() {
        $scope.isLoading = true;
        loadAllControls();
    }

    function loadAllControls() {
        return UtilsService.waitMultipleAsyncOperations([loadTimeRangeSelector, loadCustomers, loadMasterSaleZones])
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
        filter.SaleZoneIds = masterSaleZoneDirectiveAPI.getSelectedIds();
        return filter;
    }

    function loadCustomers() {
        var loadCustomerAccountPromiseDeferred = UtilsService.createPromiseDeferred();
        customerAccountReadyPromiseDeferred.promise.then(function () {
            var payload = {
            };
            VRUIUtilsService.callDirectiveLoad(customerAccountDirectiveAPI, payload, loadCustomerAccountPromiseDeferred);
        });

        return loadCustomerAccountPromiseDeferred.promise;
    }
    function loadMasterSaleZones() {
        var loadMasterSaleZonePromiseDeferred = UtilsService.createPromiseDeferred();
        masterSaleZoneReadyPromiseDeferred.promise.then(function () {
            var payload = {
                businessEntityDefinitionId:"f4505536-22bc-4382-a916-20f6e19ea041"
            };
            VRUIUtilsService.callDirectiveLoad(masterSaleZoneDirectiveAPI, payload, loadMasterSaleZonePromiseDeferred);
        });
        return loadMasterSaleZonePromiseDeferred.promise;
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