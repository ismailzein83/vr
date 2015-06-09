CDRLogController.$inject = ['$scope', 'CDRAPIService', 'UtilsService', 'uiGridConstants', '$q', 'BusinessEntityAPIService', 'BillingCDRMeasureEnum', 'TrafficStatisticsMeasureEnum', 'CarrierTypeEnum', 'VRModalService', 'VRNotificationService', 'ZonesService', 'BillingCDROptionMeasureEnum'];

function CDRLogController($scope, CDRAPIService, UtilsService, uiGridConstants, $q, BusinessEntityAPIService, BillingCDRMeasureEnum, TrafficStatisticsMeasureEnum, CarrierTypeEnum, VRModalService, VRNotificationService, ZonesService, BillingCDROptionMeasureEnum) {

    $scope.name = "test";
    $scope.isInitializing = false;

    var mainGridAPI;
    $scope.data = [];
    $scope.CDROption = [];
    $scope.durationIn = ['Min', 'Sec']
    $scope.selectedDurationIn = 'Min'
    $scope.fromDate = '2015/06/02';
    $scope.toDate = '2015/06/06';
    $scope.nRecords = '100'
    var measures = [];
    var CDROption = [];
    var sortColumn = BillingCDRMeasureEnum.Attempt;
    var sortDescending = true;
    var currentSortedColDef;
    //$scope.selectedsize = '5';
    defineScope();
    load();
    defineScopeMethods();
    function defineScope() {
        $scope.showResult = false;
        $scope.switches = [];
        $scope.selectedSwitches = [];

        $scope.customers = [];
        $scope.selectedCustomers = [];
        $scope.suppliers = [];
        $scope.selectedSuppliers = [];
        $scope.zones = [];
        $scope.selectedZones = [];

        $scope.gridAllMeasuresScope = {};
        $scope.measures = measures;
        $scope.CDROption = CDROption;

        $scope.optionsZonesFilter = {
            selectedvalues: [],
            datasource: []
        };
        
        $scope.mainGridPagerSettings = {
            currentPage: 1,
            totalDataCount: 0,
            pageChanged: function () {
                return GetCDRData();
            }
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
            //  return getData(true);
            return GetCDRData();
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
        UtilsService.waitMultipleAsyncOperations([loadSwitches, loadCustomers, loadSuppliers]).finally(function () {
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
        return filter;
    }
    function resetSorting() {
        sortColumn = BillingCDRMeasureEnum.Attempt;
        sortDescending = true;
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
            });
        });
    }
    function loadSuppliers() {
        return BusinessEntityAPIService.GetCarriers(CarrierTypeEnum.Supplier.value).then(function (response) {
            angular.forEach(response, function (itm) {
                $scope.suppliers.push(itm);
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
        var filter = buildFilter();
        //if (sortColumn == undefined)
        //    return;
        var pageInfo = $scope.mainGridPagerSettings.getPageInfo();
        var count = $scope.mainGridPagerSettings.itemsPerPage;
        var getCDRLogSummaryInput = {
            TempTableKey: null,
            Filter: filter,
            From: $scope.fromDate,
            To: $scope.toDate,
            FromRow: pageInfo.fromRow,
            ToRow: pageInfo.toRow,
            Size: $scope.nRecords,
            CDROption: $scope.selectedCDROption.value,
            OrderBy: sortColumn.value,
            IsDescending: true
        }
        $scope.showResult = true;
        $scope.isGettingData = true;
        return CDRAPIService.GetCDRData(getCDRLogSummaryInput).then(function (response) {
            //  alert(response);
            if (currentSortedColDef != undefined)
                currentSortedColDef.currentSorting = sortDescending ? 'DESC' : 'ASC';
            $scope.data = [];
            currentData = response.Data;
            resultKey = response.ResultKey;
            $scope.mainGridPagerSettings.totalDataCount = response.TotalCount;
            console.log(response);
            $scope.isInitializing = false;
            angular.forEach(response.Data, function (itm) {
                $scope.data.push(itm);
            });

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