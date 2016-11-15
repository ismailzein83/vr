"use strict";
ReleaseCodeStatisticsController.$inject = ["$scope", "UtilsService", "VRNavigationService", "WhS_BE_SaleZoneAPIService", "VRNotificationService", "VRUIUtilsService", "VRValidationService", "PeriodEnum"];

function ReleaseCodeStatisticsController($scope, UtilsService, VRNavigationService, WhS_BE_SaleZoneAPIService, VRNotificationService, VRUIUtilsService, VRValidationService, PeriodEnum) {
   
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


    var codeGroupDirectiveAPI;
    var codeGroupReadyPromiseDeferred = UtilsService.createPromiseDeferred();


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
        $scope.onCountryDirectiveReady = function (api) {
            countryDirectiveAPI = api;
            countryReadyPromiseDeferred.resolve();
        };
        $scope.onCodeGroupDirectiveReady = function (api) {
            codeGroupDirectiveAPI = api;
            codeGroupReadyPromiseDeferred.resolve();
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
        return UtilsService.waitMultipleAsyncOperations([loadTimeRangeSelector, loadReleaseCodeDimessionSection, loadCustomers, loadSuppliers, loadSwitches, /*loadCountries,*/ loadCodeGroups])
            .catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            })
            .finally(function () {
                $scope.isLoading = false;
            });
    }

    function buildFilter() {
        var filter = {};
        filter.Dimession = releaseCodeDimenssionDirectiveAPI.getSelectedIds();
        $scope.dimenssionvalues = releaseCodeDimenssionDirectiveAPI.getSelectedIds();
        filter.CustomerIds = customerDirectiveAPI.getSelectedIds();
        filter.SupplierIds = supplierDirectiveAPI.getSelectedIds();
        filter.SwitchIds = switchDirectiveAPI.getSelectedIds();
        filter.CodeGroupIds = codeGroupDirectiveAPI.getSelectedIds();
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

    function loadCodeGroups() {
        var loadCodeGroupPromiseDeferred = UtilsService.createPromiseDeferred();
        codeGroupReadyPromiseDeferred.promise.then(function () {
            VRUIUtilsService.callDirectiveLoad(codeGroupDirectiveAPI, undefined, loadCodeGroupPromiseDeferred);
        });

        return loadCodeGroupPromiseDeferred.promise;
    }



};

appControllers.controller("WhS_Analytics_ReleaseCodeStatisticsController", ReleaseCodeStatisticsController);