RawCDRLogController.$inject = ['$scope', 'CDRAPIService', 'UtilsService', 'uiGridConstants', 'VRNavigationService', '$q', 'BusinessEntityAPIService_temp', 'CarrierAccountAPIService', 'BillingCDRMeasureEnum', 'TrafficMonitorMeasureEnum', 'CarrierTypeEnum', 'VRModalService', 'VRNotificationService', 'ZonesService', 'BillingCDROptionMeasureEnum'];

function RawCDRLogController($scope, CDRAPIService, UtilsService, uiGridConstants, VRNavigationService, $q, BusinessEntityAPIService, CarrierAccountAPIService, BillingCDRMeasureEnum, TrafficMonitorMeasureEnum, CarrierTypeEnum, VRModalService, VRNotificationService, ZonesService, BillingCDROptionMeasureEnum) {

    var receivedCustomerIds;
    var receivedSupplierIds;
    var receivedZoneIds;
    var receivedSwitchIds;
    var getDataAfterLoading = false;
    var mainGridAPI;
    var measures = [];
    var CDROption = [];
    var isFilterScreenReady;
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
        if ($scope.fromDate != undefined)
            editMode = true;
        else
            editMode = false;
    }
    function defineScope() {

        $scope.ZoneIds = [];
        $scope.fromDate = '2015/06/02';
        $scope.toDate = '2015/06/06';
        $scope.nRecords = '100'
        $scope.isInitializing = false;
        $scope.switches = [];
        $scope.selectedSwitches = [];
        $scope.data = [];
        $scope.customers = [];
        $scope.selectedCustomers = [];
        $scope.suppliers = [];
        $scope.selectedSuppliers = [];
        $scope.zones = [];
        $scope.selectedZones = [];
        $scope.gridAllMeasuresScope = {};
        $scope.measures = measures;
        $scope.CDROption = CDROption;
        $scope.selectedCDROption = BillingCDROptionMeasureEnum.All.value
        $scope.onMainGridReady = function (api) {
            mainGridAPI = api;
            if (getDataAfterLoading)
                return retrieveData();
        }
        $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
            return CDRAPIService.GetCDRData(dataRetrievalInput).then(function (response) {
                onResponseReady(response);
            })
        };


        $scope.getData = function () {
            return retrieveData();
        };

        $scope.searchZones = function (text) {
            return ZonesService.getSalesZones(text);
        }

    }

    function retrieveData() {
        var filter = buildFilter();
        var query = {
            Filter: filter,
            From: $scope.fromDate,
            To: $scope.toDate,
            Size: $scope.nRecords,
            CDROption: $scope.selectedCDROption.value,
        }
        return mainGridAPI.retrieveData(query);
    }
    function load() {
        loadCDROption();
        loadMeasures();
        $scope.isInitializing = true;
        UtilsService.waitMultipleAsyncOperations([loadSwitches, loadCustomers, loadSuppliers, loadZonesFromReceivedIds])
            .then(function () {
                isFilterScreenReady = true;
                if (getDataAfterLoading && mainGridAPI != undefined)
                    return retrieveData();
            })
            .finally(function () {
                $scope.isInitializing = false;
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            });
    }
    function buildFilter() {
        var filter = {};
        filter.SwitchIds = getFilterIds($scope.selectedSwitches, "SwitchId");
        filter.CustomerIds = getFilterIds($scope.selectedCustomers, "CarrierAccountID");
        filter.SupplierIds = getFilterIds($scope.selectedSuppliers, "CarrierAccountID");
        filter.ZoneIds = getFilterIds($scope.selectedZones, "ZoneId");
        return filter;
    }
    function loadZonesFromReceivedIds() {
        if (receivedZoneIds != undefined && receivedZoneIds.length > 0) {
            return ZonesService.getZoneList(receivedZoneIds).then(function (response) {
                angular.forEach(response, function (itm) {
                    $scope.selectedZones.push(itm);
                });
            });
        }
    }
    function loadSwitches() {
        return BusinessEntityAPIService.GetSwitches().then(function (response) {
            angular.forEach(response, function (itm) {
                $scope.switches.push(itm);
                if (receivedSwitchIds != undefined && (receivedSwitchIds.indexOf(itm.SwitchId.toString()) > -1 || receivedSwitchIds.indexOf(itm.SwitchId) > -1))
                    $scope.selectedSwitches.push(itm);
            });
        });
    }
    function loadCustomers() {
        return CarrierAccountAPIService.GetCarriers(CarrierTypeEnum.Customer.value).then(function (response) {
            angular.forEach(response, function (itm) {
                $scope.customers.push(itm);
                if (receivedCustomerIds != undefined && receivedCustomerIds.indexOf(itm.CarrierAccountID) > -1)
                    $scope.selectedCustomers.push(itm);
            });
        });
    }
    function loadSuppliers() {
        return CarrierAccountAPIService.GetCarriers(CarrierTypeEnum.Supplier.value).then(function (response) {
            angular.forEach(response, function (itm) {
                $scope.suppliers.push(itm);
                if (receivedSupplierIds != undefined && receivedSupplierIds.indexOf(itm.CarrierAccountID) > -1)
                    $scope.selectedSuppliers.push(itm);
            });
        });
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
    function getFilterIds(values, idProp) {
        var filterIds;
        if (values.length > 0) {
            filterIds = [];
            angular.forEach(values, function (val) {
                filterIds.push(val[idProp]);
            });
        }
        return filterIds;
    }

};



appControllers.controller('Analytics_RawCDRLogController', RawCDRLogController);