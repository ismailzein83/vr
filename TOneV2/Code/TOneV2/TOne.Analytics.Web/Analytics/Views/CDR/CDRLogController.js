CDRLogController.$inject = ['$scope', 'CDRAPIService', 'UtilsService', 'uiGridConstants', 'VRNavigationService', '$q', 'BusinessEntityAPIService', 'BillingCDRMeasureEnum', 'TrafficStatisticsMeasureEnum', 'CarrierTypeEnum', 'VRModalService', 'VRNotificationService', 'ZonesService', 'BillingCDROptionMeasureEnum'];

function CDRLogController($scope, CDRAPIService, UtilsService, uiGridConstants,VRNavigationService, $q, BusinessEntityAPIService, BillingCDRMeasureEnum, TrafficStatisticsMeasureEnum, CarrierTypeEnum, VRModalService, VRNotificationService, ZonesService, BillingCDROptionMeasureEnum) {

    $scope.name = "test";
    $scope.isInitializing = false;
    var receivedCustomerIds;
    var receivedSupplierIds;
    var receivedZoneIds;
    var getDataAfterLoading;
    var mainGridAPI;
   
    //$scope.CDROption = [];
    $scope.fromDate = '2015/06/02';
    $scope.toDate = '2015/06/06';
    $scope.nRecords = '100'
    var measures = [];
    var CDROption = [];
    var sortDescending = true;
    var currentSortedColDef;
    $scope.ZoneIds = [];
    
    defineScope();
    loadParameters();
    var sortColumn;
    load();
    defineScopeMethods();

    function loadParameters() {
        var parameters = VRNavigationService.getParameters($scope);
        if (parameters != undefined && parameters != null) {
            $scope.fromDate = parameters.fromDate;
            $scope.toDate = parameters.toDate;
            receivedCustomerIds = parameters.customerIds;
            receivedZoneIds = parameters.zoneIds;
            receivedSupplierIds = parameters.supplierIds;

            getDataAfterLoading = true;

            console.log(receivedZoneIds);
        }
            

        if ($scope.fromDate != undefined)
            editMode = true;
        else
            editMode = false;
    }
    function defineScope() {
        
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
            return GetCDRData();
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
            //  return getData(true);
            return GetCDRData();
        };
        $scope.mainGridPagerSettings = {
            currentPage: 1,
            totalDataCount: 0,
            pageChanged: function () {
                return GetCDRData();
            }
        };
        $scope.GetCDRData = GetCDRData;

    }
    function defineScopeMethods() {
        $scope.searchZones = function (text) {
            return ZonesService.getSalesZones(text);
        }
    }
    function load() {
        loadCDROption();
        loadMeasures();
        overallSelectedMeasure = BillingCDRMeasureEnum.Attempt;
        $scope.isInitializing = true;
        UtilsService.waitMultipleAsyncOperations([loadSwitches, loadCustomers, loadSuppliers, loadZonesFromReceivedIds])
            .then(function () {
                if (getDataAfterLoading)
                    $scope.getData();
            })
            .finally(function () {
            $scope.isInitializing = false;
        }).catch(function (error) {
            VRNotificationService.notifyExceptionWithClose(error);
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
            });
        });
    }
    function loadCustomers() {
        return BusinessEntityAPIService.GetCarriers(CarrierTypeEnum.Customer.value).then(function (response) {
            angular.forEach(response, function (itm) {
                $scope.customers.push(itm);
                if (receivedCustomerIds != undefined && receivedCustomerIds.indexOf(itm.CarrierAccountID)>-1)
                    $scope.selectedCustomers.push(itm);
            });
        });
    }
    function loadSuppliers() {
        return BusinessEntityAPIService.GetCarriers(CarrierTypeEnum.Supplier.value).then(function (response) {
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
    //function loadZones() {
    //    return BusinessEntityAPIService.GetZones("1","MTN").then(function (response) {
    //        angular.forEach(response, function (itm) {
    //            $scope.zones.push(itm);
    //        });
    //    });
    //}
    function GetCDRData() {
      
        if (sortColumn == undefined)
            return;
        //if (sortColumn == undefined)
        //    return;
        
        var pageInfo = $scope.mainGridPagerSettings.getPageInfo();
        var count = $scope.mainGridPagerSettings.itemsPerPage;
        var getCDRLogSummaryInput = {
            //ZoneIDs: $scope.zoneIDs,
            TempTableKey: null,
            Filter: $scope.filter.filter,
            From: $scope.filter.fromDate,
            To: $scope.filter.toDate,
            FromRow: pageInfo.fromRow,
            ToRow: pageInfo.toRow,
            Size: $scope.filter.nRecords,
            CDROption: $scope.filter.selectedCDROption,
            OrderBy: sortColumn.value,
            IsDescending: true
            
        }
        $scope.showResult = true;
        $scope.isGettingData = true;
        
        return CDRAPIService.GetCDRData(getCDRLogSummaryInput).then(function (response) {
            //  alert(response);
            if (currentSortedColDef != undefined)
                currentSortedColDef.currentSorting = sortDescending ? 'DESC' : 'ASC';
            $scope.data.length = 0;
            currentData = response.Data;
            resultKey = response.ResultKey;
            $scope.mainGridPagerSettings.totalDataCount = response.TotalCount;
            console.log(response);
            $scope.isInitializing = false;
            //angular.forEach(response.Data, function (itm) {
            //    $scope.data.push(itm);
            //});
            mainGridAPI.addItemsToSource(response.Data);

        }).finally(function () {
            $scope.isGettingData = false;
        });
    }
   

};

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


appControllers.controller('Analytics_CDRLogController', CDRLogController);