﻿"use strict";
ReleaseCodeStatisticsController.$inject = ["$scope", "UtilsService", "VRNavigationService", "VRNotificationService", "VRUIUtilsService", "VRValidationService", "PeriodEnum", "CP_WhSAnalytics_AccountViewTypeEnum"];

function ReleaseCodeStatisticsController($scope, UtilsService, VRNavigationService, VRNotificationService, VRUIUtilsService, VRValidationService, PeriodEnum, CP_WhSAnalytics_AccountViewTypeEnum) {

    var mainGridAPI;

    var timeRangeDirectiveAPI;
    var timeRangeReadyPromiseDeferred = UtilsService.createPromiseDeferred();

    var releaseCodeDimenssionDirectiveAPI;
    var releaseCodReadyPromiseDeferred = UtilsService.createPromiseDeferred();

    var customerDirectiveAPI;
    var customerReadyPromiseDeferred = UtilsService.createPromiseDeferred();

    var supplierDirectiveAPI;
    var supplierReadyPromiseDeferred = UtilsService.createPromiseDeferred();

    var switchDirectiveAPI;
    var switchReadyPromiseDeferred = UtilsService.createPromiseDeferred();

    var countryDirectiveAPI;
    var countryReadyPromiseDeferred = UtilsService.createPromiseDeferred();

    var saleZoneDirectiveAPI;
    var saleZoneReadyPromiseDeferred = UtilsService.createPromiseDeferred();

    var accountViewTypeAPI;
    var accountViewTypeReadyPromiseDeferred = UtilsService.createPromiseDeferred();

    defineScope();
    load();

    function defineScope() {
        $scope.fromDate;
        $scope.toDate;
        $scope.dimenssionvalues = [];
        $scope.today = PeriodEnum.Today;

        $scope.onTimeRangeDirectiveReady = function (api) {
            timeRangeDirectiveAPI = api;
            timeRangeReadyPromiseDeferred.resolve();
        };
        $scope.onReleaseCodeDimenssionDirectiveReady = function (api) {
            releaseCodeDimenssionDirectiveAPI = api;
            releaseCodReadyPromiseDeferred.resolve();
        };

        $scope.onCustomerDirectiveReady = function (api) {
            customerDirectiveAPI = api;
            customerReadyPromiseDeferred.resolve();
        };
        $scope.onSupplierDirectiveReady = function (api) {
            supplierDirectiveAPI = api;
            supplierReadyPromiseDeferred.resolve();
        };

        $scope.onSwitchDirectiveReady = function (api) {
            switchDirectiveAPI = api;
            switchReadyPromiseDeferred.resolve();
        };
        $scope.onCountryReady = function (api) {
            countryDirectiveAPI = api;
            countryReadyPromiseDeferred.resolve();
        };
        $scope.onSaleZoneDirectiveReady = function (api) {
            saleZoneDirectiveAPI = api;
            saleZoneReadyPromiseDeferred.resolve();
        };
        $scope.onAccountViewTypeSelectorReady = function (api) {
            accountViewTypeAPI = api;
            accountViewTypeReadyPromiseDeferred.resolve();
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
            To: $scope.toDate
        };
        return query;
    }

    function load() {
        $scope.isLoading = true;
        loadAllControls();
    }

    function loadAllControls() {
        return UtilsService.waitMultipleAsyncOperations([loadTimeRangeSelector, loadReleaseCodeDimessionSection, loadAccountViewTypeSelector])
            .catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            })
            .finally(function () {
                $scope.isLoading = false;
            });
    }

    function buildFilter() {
        var filter = {};
        //filter.Dimession = releaseCodeDimenssionDirectiveAPI.getSelectedIds();
        //$scope.dimenssionvalues = releaseCodeDimenssionDirectiveAPI.getSelectedIds();
        //filter.CustomerIds = customerDirectiveAPI.getSelectedIds();
        //filter.SupplierIds = supplierDirectiveAPI.getSelectedIds();
        //filter.SwitchIds = switchDirectiveAPI.getSelectedIds();
        //filter.CountryIds = countryDirectiveAPI.getSelectedIds();
        //filter.MasterSaleZoneIds = saleZoneDirectiveAPI.getSelectedIds();

        return filter;
    }
    function loadTimeRangeSelector() {
        var timeRangeLoadPromiseDeferred = UtilsService.createPromiseDeferred();
        timeRangeReadyPromiseDeferred.promise.then(function () {
            VRUIUtilsService.callDirectiveLoad(timeRangeDirectiveAPI, undefined, timeRangeLoadPromiseDeferred);
        });
        return timeRangeLoadPromiseDeferred.promise;
    }
    function loadReleaseCodeDimessionSection() {
        var loadReleaseCodeDimessionPromiseDeferred = UtilsService.createPromiseDeferred();
        releaseCodReadyPromiseDeferred.promise.then(function () {
            VRUIUtilsService.callDirectiveLoad(releaseCodeDimenssionDirectiveAPI, undefined, loadReleaseCodeDimessionPromiseDeferred);
        });
        return loadReleaseCodeDimessionPromiseDeferred.promise;
    }
    function loadAccountViewTypeSelector() {
        var accountViewTypeLoadPromiseDeferred = UtilsService.createPromiseDeferred();
        accountViewTypeReadyPromiseDeferred.promise.then(function () {
            VRUIUtilsService.callDirectiveLoad(accountViewTypeAPI, undefined, accountViewTypeLoadPromiseDeferred);
        });
        return accountViewTypeLoadPromiseDeferred.promise;
    }

    function loadSwitches() {
        var loadSwitchPromiseDeferred = UtilsService.createPromiseDeferred();
        switchReadyPromiseDeferred.promise.then(function () {
            VRUIUtilsService.callDirectiveLoad(switchDirectiveAPI, undefined, loadSwitchPromiseDeferred);
        });
        return loadSwitchPromiseDeferred.promise;
    }

    function loadCustomers() {
        var loadCustomerPromiseDeferred = UtilsService.createPromiseDeferred();
        customerReadyPromiseDeferred.promise.then(function () {
            VRUIUtilsService.callDirectiveLoad(customerDirectiveAPI, undefined, loadCustomerPromiseDeferred);
        });

        return loadCustomerPromiseDeferred.promise;
    }
    function loadSuppliers() {
        var loadSupplierPromiseDeferred = UtilsService.createPromiseDeferred();
        supplierReadyPromiseDeferred.promise.then(function () {
            VRUIUtilsService.callDirectiveLoad(supplierDirectiveAPI, undefined, loadSupplierPromiseDeferred);
        });

        return loadSupplierPromiseDeferred.promise;
    }

    function loadCountries() {
        var loadCountryPromiseDeferred = UtilsService.createPromiseDeferred();
        countryReadyPromiseDeferred.promise.then(function () {
            VRUIUtilsService.callDirectiveLoad(countryDirectiveAPI, undefined, loadCountryPromiseDeferred);
        });

        return loadCountryPromiseDeferred.promise;
    }

    //function loadCodeGroups() {
    //    var loadCodeGroupPromiseDeferred = UtilsService.createPromiseDeferred();
    //    codeGroupReadyPromiseDeferred.promise.then(function () {
    //        VRUIUtilsService.callDirectiveLoad(codeGroupDirectiveAPI, undefined, loadCodeGroupPromiseDeferred);
    //    });

    //    return loadCodeGroupPromiseDeferred.promise;
    //}


    function loadSaleZoneSection() {
        var loadSaleZonePromiseDeferred = UtilsService.createPromiseDeferred();
        saleZoneReadyPromiseDeferred.promise.then(function () {
            VRUIUtilsService.callDirectiveLoad(saleZoneDirectiveAPI, undefined, loadSaleZonePromiseDeferred);
        });
        return loadSaleZonePromiseDeferred.promise;
    }


};

appControllers.controller("CP_WhSAnalytics_ReleaseCodeStatisticsController", ReleaseCodeStatisticsController);