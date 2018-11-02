"use strict";
ReleaseCodeStatisticsController.$inject = ["$scope", "UtilsService", "VRNavigationService", "VRNotificationService", "VRUIUtilsService", "VRValidationService", "PeriodEnum", "CP_WhS_AccountViewTypeEnum"];

function ReleaseCodeStatisticsController($scope, UtilsService, VRNavigationService, VRNotificationService, VRUIUtilsService, VRValidationService, PeriodEnum, CP_WhS_AccountViewTypeEnum) {

    var mainGridAPI;

    var timeRangeDirectiveAPI;
    var timeRangeReadyPromiseDeferred = UtilsService.createPromiseDeferred();

    var releaseCodeDimensionDirectiveAPI;
    var releaseCodReadyPromiseDeferred = UtilsService.createPromiseDeferred();

    var accountDirectiveAPI;
    var accountReadyPromiseDeferred = UtilsService.createPromiseDeferred();

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
        $scope.dimensionvalues = [];
        $scope.today = PeriodEnum.Today;

        $scope.onTimeRangeDirectiveReady = function (api) {
            timeRangeDirectiveAPI = api;
            timeRangeReadyPromiseDeferred.resolve();
        };
        $scope.onReleaseCodeDimenssionDirectiveReady = function (api) {
            releaseCodeDimensionDirectiveAPI = api;
            releaseCodReadyPromiseDeferred.resolve();
        };

      
        $scope.onAccountDirectiveReady = function (api) {
            accountDirectiveAPI = api;
            accountReadyPromiseDeferred.resolve();
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

        $scope.onAccountViewTypeSelectionChange = function (value) {
            if (value != undefined) {
                var setLoader = function (value) {
                    $scope.isLoading = value;
                };
                var accountViewTypePayload = {
                    businessEntityDefinitionId: accountViewTypeAPI.getSelectedIds() == CP_WhS_AccountViewTypeEnum.Customer.value ? "32cc6b0b-785c-437c-9a58-bab8753e50ee" : "574ef14e-64d3-4f56-8d0a-8ec05f043b7b"
                };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, accountDirectiveAPI, accountViewTypePayload, setLoader, undefined);

                var dimensionsPayload = {
                    hideSupplier: accountViewTypeAPI.getSelectedIds() == CP_WhS_AccountViewTypeEnum.Customer.value
                };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, releaseCodeDimensionDirectiveAPI, dimensionsPayload, setLoader, undefined);
            }
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
        return UtilsService.waitMultipleAsyncOperations([loadTimeRangeSelector, loadAccountViewTypeSelector])
            .catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            })
            .finally(function () {
                $scope.isLoading = false;
            });
    }

    function buildFilter() {
        var filter = {};
        filter.Dimession = releaseCodeDimensionDirectiveAPI.getSelectedIds();
        $scope.dimensionvalues = filter.Dimession;
        if (accountViewTypeAPI.getSelectedIds() == CP_WhS_AccountViewTypeEnum.Customer.value) {
            filter.CustomerIds = accountDirectiveAPI.getSelectedIds();
        }
        else {
            filter.SupplierIds = accountDirectiveAPI.getSelectedIds();
        }
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

    function loadAccountViewTypeSelector() {
        var accountViewTypeLoadPromiseDeferred = UtilsService.createPromiseDeferred();
        accountViewTypeReadyPromiseDeferred.promise.then(function () {
            VRUIUtilsService.callDirectiveLoad(accountViewTypeAPI, undefined, accountViewTypeLoadPromiseDeferred);
        });
        return accountViewTypeLoadPromiseDeferred.promise;
    }

    function loadCountries() {
        var loadCountryPromiseDeferred = UtilsService.createPromiseDeferred();
        countryReadyPromiseDeferred.promise.then(function () {
            VRUIUtilsService.callDirectiveLoad(countryDirectiveAPI, undefined, loadCountryPromiseDeferred);
        });

        return loadCountryPromiseDeferred.promise;
    }


    function loadSaleZoneSection() {
        var loadSaleZonePromiseDeferred = UtilsService.createPromiseDeferred();
        saleZoneReadyPromiseDeferred.promise.then(function () {
            VRUIUtilsService.callDirectiveLoad(saleZoneDirectiveAPI, undefined, loadSaleZonePromiseDeferred);
        });
        return loadSaleZonePromiseDeferred.promise;
    }
};

appControllers.controller("CP_WhS_ReleaseCodeStatisticsController", ReleaseCodeStatisticsController);