CDRLogController.$inject = ['$scope', 'CDRAPIService', 'UtilsService', 'uiGridConstants', 'VRNavigationService', '$q', 'BusinessEntityAPIService_temp', 'CarrierAPIService', 'BillingCDRMeasureEnum', 'TrafficStatisticsMeasureEnum', 'CarrierTypeEnum', 'VRModalService', 'VRNotificationService', 'ZonesService', 'BillingCDROptionMeasureEnum'];

function CDRLogController($scope, CDRAPIService, UtilsService, uiGridConstants, VRNavigationService, $q, BusinessEntityAPIService, CarrierAPIService, BillingCDRMeasureEnum, TrafficStatisticsMeasureEnum, CarrierTypeEnum, VRModalService, VRNotificationService, ZonesService, BillingCDROptionMeasureEnum) {

    var receivedCustomerIds;
    var receivedSupplierIds;
    var receivedZoneIds;
    var receivedSwitchIds;
    var getDataAfterLoading;
    var mainGridAPI;
    var measures = [];
    var CDROption = [];
    var sortDescending = true;
    var currentSortedColDef;
    var sortColumn;
    var resultKey;
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
        $scope.showResult = false;
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
        $scope.filter = {
            filter: {},
            fromDate: $scope.fromDate,
            toDate: $scope.toDate,
            nRecords: $scope.nRecords,
            selectedCDROption: $scope.selectedCDROption.value
        };
        $scope.onMainGridReady = function (api) {
            mainGridAPI = api;
        }
        $scope.onMainGridSortChanged = function (colDef, sortDirection) {
            sortColumn = colDef.tag;
            sortDescending = (sortDirection == "DESC");
            return getData();
        }

   
        $scope.getData = function () {
            $scope.mainGridPagerSettings.currentPage = 1;
            resultKey = null;
            mainGridAPI.resetSorting();
            resetSorting();
            var  filter= buildFilter();
            $scope.filter = {
                filter: filter,
                fromDate: $scope.fromDate,
                toDate: $scope.toDate,
                nRecords: $scope.nRecords,
                selectedCDROption: $scope.selectedCDROption.value
            };
            return getData();
        };
        $scope.mainGridPagerSettings = {
            currentPage: 1,
            totalDataCount: 0,
            pageChanged: function () {
                return getData();
            }
        };

        $scope.searchZones = function (text) {
            return ZonesService.getSalesZones(text);
        }
        $scope.onexport = function () {
            var pageInfo = $scope.mainGridPagerSettings.getPageInfo();
            var count = $scope.mainGridPagerSettings.itemsPerPage;
            var CDRLogSummaryInput = {
                TempTableKey: resultKey,
                Filter: $scope.filter.filter,
                From: $scope.filter.fromDate,
                To: $scope.filter.toDate,
                FromRow: pageInfo.fromRow,
                ToRow: pageInfo.toRow,
                Size: $scope.filter.nRecords,
                CDROption: $scope.filter.selectedCDROption,
                OrderBy: sortColumn.value,
                IsDescending: sortDescending

            }
            return CDRAPIService.ExportCDRData(CDRLogSummaryInput).then(function (response) {
                return UtilsService.downloadFile(response.data, response.headers, response.config);
            });
        }
    }
    function load() {
        loadCDROption();
        loadMeasures();
        $scope.isInitializing = true;
        UtilsService.waitMultipleAsyncOperations([loadSwitches, loadCustomers, loadSuppliers, loadZonesFromReceivedIds])
            .then(function () {
                if (getDataAfterLoading)
                    $scope.getData();
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
    function resetSorting() {
        sortColumn = BillingCDRMeasureEnum.Attempt;
        sortDescending = true;
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
                if (receivedSwitchIds != undefined && receivedSwitchIds.indexOf(itm.SwitchId.toString()) > -1)
                    $scope.selectedSwitches.push(itm);
            });
        });
    }
    function loadCustomers() {
        return CarrierAPIService.GetCarriers(CarrierTypeEnum.Customer.value).then(function (response) {
            angular.forEach(response, function (itm) {
                $scope.customers.push(itm);
                if (receivedCustomerIds != undefined && receivedCustomerIds.indexOf(itm.CarrierAccountID)>-1)
                    $scope.selectedCustomers.push(itm);
            });
        });
    }
    function loadSuppliers() {
        return CarrierAPIService.GetCarriers(CarrierTypeEnum.Supplier.value).then(function (response) {
            angular.forEach(response, function (itm) {
                $scope.suppliers.push(itm);
                if(receivedSupplierIds !=undefined && receivedSupplierIds.indexOf(itm.CarrierAccountID)>-1)
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
    function getData() {
        if (sortColumn == undefined)
            return;
        var pageInfo = $scope.mainGridPagerSettings.getPageInfo();
        var count = $scope.mainGridPagerSettings.itemsPerPage;
        var getCDRLogSummaryInput = {
            TempTableKey: resultKey,
            Filter: $scope.filter.filter,
            From: $scope.filter.fromDate,
            To: $scope.filter.toDate,
            FromRow: pageInfo.fromRow,
            ToRow: pageInfo.toRow,
            Size: $scope.filter.nRecords,
            CDROption: $scope.filter.selectedCDROption,
            OrderBy: sortColumn.value,
            IsDescending: sortDescending
            
        }
        $scope.showResult = true;
        $scope.isGettingData = true;
        
        return CDRAPIService.GetCDRData(getCDRLogSummaryInput).then(function (response) {
            if (currentSortedColDef != undefined)
                currentSortedColDef.currentSorting = sortDescending ? 'DESC' : 'ASC';
            $scope.data.length = 0;
            currentData = response.Data;
            resultKey = response.ResultKey;
            $scope.mainGridPagerSettings.totalDataCount = response.TotalCount;
            $scope.isInitializing = false;
            mainGridAPI.addItemsToSource(response.Data);
        }).finally(function () {
            $scope.isGettingData = false;
        });
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



appControllers.controller('Analytics_CDRLogController', CDRLogController);