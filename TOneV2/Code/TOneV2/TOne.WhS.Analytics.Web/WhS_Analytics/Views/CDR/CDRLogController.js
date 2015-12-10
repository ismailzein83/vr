CDRLogController.$inject = ['$scope', 'CDRAPIService', 'UtilsService', 'uiGridConstants', 'VRNavigationService', '$q', 'BusinessEntityAPIService_temp', 'CarrierAccountAPIService', 'WhS_Analytics_BillingCDRMeasureEnum', 'TrafficMonitorMeasureEnum', 'CarrierTypeEnum', 'VRModalService', 'VRNotificationService', 'ZonesService', 'WhS_Analytics_BillingCDROptionMeasureEnum','VRUIUtilsService','VRValidationService'];

function CDRLogController($scope, CDRAPIService, UtilsService, uiGridConstants, VRNavigationService, $q, BusinessEntityAPIService, CarrierAccountAPIService, BillingCDRMeasureEnum, TrafficMonitorMeasureEnum, CarrierTypeEnum, VRModalService, VRNotificationService, ZonesService, BillingCDROptionMeasureEnum, VRUIUtilsService, VRValidationService) {

    var receivedCustomerIds;
    var receivedSupplierIds;
    var receivedZoneIds;
    var receivedSwitchIds;
    var getDataAfterLoading=false;
    var mainGridAPI;
    var measures = [];
    var isEditMode = false;
    var CDROption = [];
    var isFilterScreenReady;


    var cdrLogEntity;


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
            receivedZoneIds = parameters.zoneIds;
            receivedSupplierIds = parameters.supplierIds;
            receivedSwitchIds = parameters.switchIds;
            getDataAfterLoading = true;
        }
        //if ($scope.fromDate != undefined)
        //    isEditMode = true;
        //else
            isEditMode = false;
    }
    function defineScope() {
        

        $scope.validateDateTime = function () {
            return VRValidationService.validateTimeRange($scope.fromDate, $scope.toDate);
        }
        

        $scope.onSupplierZoneDirectiveReady = function (api) {
            supplierZoneDirectiveAPI = api;
            supplierZoneReadyPromiseDeferred.resolve();
        }

        $scope.onCustomerAccountDirectiveReady = function (api) {
            customerAccountDirectiveAPI = api;
            customerAccountReadyPromiseDeferred.resolve();
        }

        $scope.onSwitchDirectiveReady = function (api) {
            switchDirectiveAPI = api;
            switchReadyPromiseDeferred.resolve();
        }

        $scope.onSupplierAccountDirectiveReady = function (api) {
            supplierAccountDirectiveAPI = api;
            supplierAccountReadyPromiseDeferred.resolve();
        }

        $scope.onSellingNumberPlanDirectiveReady = function (api) {
            sellingNumberPlanDirectiveAPI = api;
            sellingNumberPlanReadyPromiseDeferred.resolve();
        }

        $scope.onSaleZoneDirectiveReady = function (api) {
            saleZoneDirectiveAPI = api;
            saleZoneReadyPromiseDeferred.resolve();
        }

        $scope.onSelectSellingNumberPlan = function (selectedItem) {
            $scope.showSaleZoneSelector = true;

            var payload = {
                sellingNumberPlanId: selectedItem.SellingNumberPlanId,
            }

            var setLoader = function (value) { $scope.isLoadingSaleZonesSection = value };
            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, saleZoneDirectiveAPI, payload, setLoader);
        }

        $scope.onSelectSupplier = function (selectedItem) {
                var payload = {
                    filter: { SupplierId: selectedItem.CarrierAccountId },
                }
                var setLoader = function (value) { $scope.isLoadingSupplierZonesSection = value };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, supplierZoneDirectiveAPI, payload, setLoader);
        }
        $scope.nRecords = '100'

        var date=new Date();
        $scope.fromDate=new Date(date.getFullYear(),date.getMonth(),date.getDate(),00,00,00,00);
        $scope.toDate = new Date(date.getFullYear(), date.getMonth(), date.getDate(), 23, 59, 00, 00);
        $scope.gridAllMeasuresScope = {};
        $scope.measures = measures;
        $scope.CDROption = CDROption;
        $scope.selectedCDROption = BillingCDROptionMeasureEnum.All.value

        $scope.onMainGridReady = function (api) {
            mainGridAPI = api;
        }

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
        }
        return query;
    }

    function load() {
        $scope.isLoading = true;
        if (isEditMode) {
            //getCDRLog().then(function () {
            //    loadAllControls()
            //        .finally(function () {
            //            cdrLogEntity = undefined;
            //        });
            //}).catch(function () {
            //    VRNotificationService.notifyExceptionWithClose(error, $scope);
            //    $scope.isLoading = false;
            //});
        }
        else {
            loadAllControls();
        }
    }

    function loadAllControls() {
        return UtilsService.waitMultipleAsyncOperations([loadCustomers, loadSuppliers, loadSellingNumberPlanSection, loadCDROption, loadMeasures, loadSwitches])
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
            VRUIUtilsService.callDirectiveLoad(switchDirectiveAPI, undefined, loadSwitchPromiseDeferred);
        });
        return loadSwitchPromiseDeferred.promise;
    }

    function loadSellingNumberPlanSection() {
        var loadSellingNumberPlanPromiseDeferred = UtilsService.createPromiseDeferred();
        sellingNumberPlanReadyPromiseDeferred.promise.then(function () {
            VRUIUtilsService.callDirectiveLoad(sellingNumberPlanDirectiveAPI, undefined, loadSellingNumberPlanPromiseDeferred);
        });
        return loadSellingNumberPlanPromiseDeferred.promise;
    }

    function loadCustomers() {
        var loadCustomerAccountPromiseDeferred = UtilsService.createPromiseDeferred();
        customerAccountReadyPromiseDeferred.promise.then(function () {
            VRUIUtilsService.callDirectiveLoad(customerAccountDirectiveAPI, undefined, loadCustomerAccountPromiseDeferred);
        });

        return loadCustomerAccountPromiseDeferred.promise;
    }

    function loadSuppliers() {
        var loadSupplierAccountPromiseDeferred = UtilsService.createPromiseDeferred();
        supplierAccountReadyPromiseDeferred.promise.then(function () {
            VRUIUtilsService.callDirectiveLoad(supplierAccountDirectiveAPI, undefined, loadSupplierAccountPromiseDeferred);
        });
        return loadSupplierAccountPromiseDeferred.promise;
    }

    function loadMeasures() {
        for (var prop in BillingCDRMeasureEnum) {
            measures.push(BillingCDRMeasureEnum[prop]);
        }
    }

    function loadCDROption() {
        for (var prop in BillingCDROptionMeasureEnum) {
            CDROption.push(BillingCDROptionMeasureEnum[prop]);
        }
        $scope.selectedCDROption = CDROption[2];
    }


};



appControllers.controller('WhS_Analytics_CDRLogController', CDRLogController);