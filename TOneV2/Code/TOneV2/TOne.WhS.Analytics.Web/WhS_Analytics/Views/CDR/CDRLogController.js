"use strict";
CDRLogController.$inject = ['$scope', 'UtilsService', 'VRNavigationService', '$q', 'WhS_BE_SaleZoneAPIService', 'VRNotificationService', 'WhS_Analytics_BillingCDROptionMeasureEnum', 'VRUIUtilsService', 'VRValidationService'];

function CDRLogController($scope, UtilsService, VRNavigationService, $q, WhS_BE_SaleZoneAPIService, VRNotificationService, BillingCDROptionMeasureEnum, VRUIUtilsService, VRValidationService) {

    var receivedCustomerIds;
    var receivedSupplierIds;
    var receivedSaleZoneIds;
    var receivedSwitchIds;
    var receivedSupplierZoneIds;
    var sellingNumberPlanId;
    var mainGridAPI;
    var isModalMode = false;

    var customerAccountDirectiveAPI;
    var customerAccountReadyPromiseDeferred = UtilsService.createPromiseDeferred();

    var supplierAccountDirectiveAPI;
    var supplierAccountReadyPromiseDeferred = UtilsService.createPromiseDeferred();

    var sellingNumberPlanDirectiveAPI;
    var sellingNumberPlanReadyPromiseDeferred = UtilsService.createPromiseDeferred();

    var saleZoneDirectiveAPI;
    var saleZoneReadyPromiseDeferred = UtilsService.createPromiseDeferred();

    var switchDirectiveAPI;
    var switchReadyPromiseDeferred = UtilsService.createPromiseDeferred();

    var supplierZoneDirectiveAPI;
    var supplierZoneReadyPromiseDeferred = UtilsService.createPromiseDeferred();

    defineScope();
    loadParameters();
    load();

    function loadParameters() {
        var parameters = VRNavigationService.getParameters($scope);
        if (parameters != undefined && parameters != null) {
            $scope.fromDate = parameters.fromDate;
            $scope.toDate = parameters.toDate;
            receivedCustomerIds = parameters.customerIds;
            receivedSaleZoneIds = parameters.saleZoneIds;
            receivedSupplierIds = parameters.supplierIds;
            receivedSwitchIds = parameters.switchIds;
            receivedSupplierZoneIds = parameters.supplierZoneIds;
            isModalMode = true;
            $scope.advancedSelected = true;
        }
    }

    function defineScope() {

        $scope.validateDateTime = function () {
            return VRValidationService.validateTimeRange($scope.fromDate, $scope.toDate);
        };

        $scope.onSupplierZoneDirectiveReady = function (api) {
            supplierZoneDirectiveAPI = api;
            supplierZoneReadyPromiseDeferred.resolve();
        };

        $scope.onCustomerAccountDirectiveReady = function (api) {
            customerAccountDirectiveAPI = api;
            customerAccountReadyPromiseDeferred.resolve();
        };

        $scope.onSwitchDirectiveReady = function (api) {
            switchDirectiveAPI = api;
            switchReadyPromiseDeferred.resolve();
        };

        $scope.onSupplierAccountDirectiveReady = function (api) {
            supplierAccountDirectiveAPI = api;
            supplierAccountReadyPromiseDeferred.resolve();
        };

        $scope.onSellingNumberPlanDirectiveReady = function (api) {
            sellingNumberPlanDirectiveAPI = api;
            sellingNumberPlanReadyPromiseDeferred.resolve();
        };

        $scope.onSaleZoneDirectiveReady = function (api) {
            saleZoneDirectiveAPI = api;
            saleZoneReadyPromiseDeferred.resolve();
        };

        $scope.onSelectSellingNumberPlan = function () {
            if ($scope.selectedSellingNumberPlan != undefined) {
                $scope.showSaleZoneSelector = true;

                var payload = {
                    sellingNumberPlanId: $scope.selectedSellingNumberPlan.SellingNumberPlanId,
                    selectedIds: receivedSaleZoneIds
                };
                var setLoader = function (value) {
                    $scope.isLoadingSaleZonesSection = value
                };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, saleZoneDirectiveAPI, payload, setLoader);
            }
        };

        $scope.useOneTime = true;
        $scope.onSelectSupplier = function () {

            if ($scope.selectedSuppliers.length == 1 && $scope.useOneTime) {
                var payload = {
                    filter: {
                        SupplierId: $scope.selectedSuppliers[0].CarrierAccountId
                    },
                    selectedIds: receivedSupplierZoneIds
                };
                var setLoader = function (value) {
                    $scope.isLoadingSupplierZonesSection = value;
                };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, supplierZoneDirectiveAPI, payload, setLoader);
            }
            $scope.useOneTime = !$scope.useOneTime;
        };

        $scope.nRecords = '100';

        var date = new Date();
        $scope.fromDate = new Date(date.getFullYear(), date.getMonth(), date.getDate(), 0, 0, 0, 0);

        $scope.CDROption = [];
        $scope.selectedCDROption = BillingCDROptionMeasureEnum.All.value;

        $scope.onMainGridReady = function (api) {
            mainGridAPI = api;
            if (isModalMode && !$scope.isLoading) {
                mainGridAPI.loadGrid(getQuery());
            }
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
            NRecords: $scope.nRecords,
            CDRType: $scope.selectedCDROption.value,
        };
        return query;
    }

    function load() {
        $scope.isLoading = true;
        if (isModalMode && (receivedSaleZoneIds != undefined) && receivedSaleZoneIds.length > 0) {
            return WhS_BE_SaleZoneAPIService.GetSaleZone(receivedSaleZoneIds[0])
                .then(function (response) {
                    if (response!=null)
                        sellingNumberPlanId = response.SellingNumberPlanId;
                    loadAllControls();
                });
        } else {
            loadAllControls();
        }
    }

    function loadAllControls() {
        return UtilsService.waitMultipleAsyncOperations([loadCustomers, loadSuppliers, loadSellingNumberPlanSection, loadCDROption, loadSwitches])
            .then(function () {

                if (isModalMode && mainGridAPI != undefined)
                    mainGridAPI.loadGrid(getQuery());
            })
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
        filter.SupplierIds = supplierAccountDirectiveAPI.getSelectedIds();
        filter.SaleZoneIds = saleZoneDirectiveAPI.getSelectedIds();
        filter.SupplierZoneIds = supplierZoneDirectiveAPI.getSelectedIds();
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

    function loadSellingNumberPlanSection() {
        var loadSellingNumberPlanPromiseDeferred = UtilsService.createPromiseDeferred();
        sellingNumberPlanReadyPromiseDeferred.promise.then(function () {
            var payload = {
                selectedIds: sellingNumberPlanId
            };
            VRUIUtilsService.callDirectiveLoad(sellingNumberPlanDirectiveAPI, payload, loadSellingNumberPlanPromiseDeferred);
        });
        return loadSellingNumberPlanPromiseDeferred.promise;
    }

    function loadCustomers() {
        var loadCustomerAccountPromiseDeferred = UtilsService.createPromiseDeferred();
        customerAccountReadyPromiseDeferred.promise.then(function () {
            var payload = {
                selectedIds: receivedCustomerIds
            };

            VRUIUtilsService.callDirectiveLoad(customerAccountDirectiveAPI, payload, loadCustomerAccountPromiseDeferred);
        });

        return loadCustomerAccountPromiseDeferred.promise;
    }

    function loadSuppliers() {
        var loadSupplierAccountPromiseDeferred = UtilsService.createPromiseDeferred();
        supplierAccountReadyPromiseDeferred.promise.then(function () {
            var payload = {
                selectedIds: receivedSupplierIds
            };
            VRUIUtilsService.callDirectiveLoad(supplierAccountDirectiveAPI, payload, loadSupplierAccountPromiseDeferred);
        });
        return loadSupplierAccountPromiseDeferred.promise;
    }

    function loadCDROption() {
        for (var prop in BillingCDROptionMeasureEnum) {
            $scope.CDROption.push(BillingCDROptionMeasureEnum[prop]);
        }
        $scope.selectedCDROption = BillingCDROptionMeasureEnum.All;
    }

};

appControllers.controller('WhS_Analytics_CDRLogController', CDRLogController);