CDRLogController.$inject = ['$scope', 'CDRAPIService', 'UtilsService', 'uiGridConstants', '$q', 'BusinessEntityAPIService', 'TrafficStatisticsMeasureEnum', 'CarrierTypeEnum', 'VRModalService', 'VRNotificationService', 'ZonesService'];

function CDRLogController($scope, CDRAPIService, UtilsService, uiGridConstants, $q, BusinessEntityAPIService, TrafficStatisticsMeasureEnum, CarrierTypeEnum, VRModalService, VRNotificationService, ZonesService) {

    $scope.name = "test";
    $scope.isInitializing = false;

    var mainGridAPI;
    $scope.data = [];
    $scope.Size = ['5', '10', '20', '30']
    $scope.CDROption = ['All', 'Successful', 'Failed']
    $scope.selectedCDROption = 'All'
    $scope.durationIn = ['Min', 'Sec']
    $scope.selectedDurationIn = 'Min'
    $scope.fromDate = '2015/06/02';
    $scope.toDate = '2015/06/06';
    $scope.nRecords='5'
    $scope.selectedsize = '5';
    defineScope();
    load();
    defineScopeMethods();
    function defineScope() {
        $scope.switches = [];
        $scope.selectedSwitches = [];

        $scope.customers = [];
        $scope.selectedCustomers = [];
        $scope.suppliers = [];
        $scope.selectedSuppliers = [];
        $scope.zones = [];
        $scope.selectedZones = [];

        $scope.optionsZonesFilter = {
            selectedvalues: [],
            datasource: []
        };


        $scope.mainGridPagerSettings = {
            currentPage: 1,
            totalDataCount: 0,
            pageChanged: function () {
                //      return getData();
            }
        };
        $scope.onMainGridReady = function (api) {
            mainGridAPI = api;
        }

        $scope.getData = function () {
            $scope.mainGridPagerSettings.currentPage = 1;
            resultKey = null;
            //    mainGridAPI.resetSorting();
            //    resetSorting();
            //  return getData(true);
            return getData(true);
        };

        $scope.GetCDRData = GetCDRData;

    }
    function defineScopeMethods() {
        $scope.searchZones = function (text) {
            return ZonesService.getSalesZones(text);
        }
    }
    function load() {
        
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
        
        return filter;
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
    //function loadZones() {
    //    return BusinessEntityAPIService.GetZones("1","MTN").then(function (response) {
    //        angular.forEach(response, function (itm) {
    //            $scope.zones.push(itm);
    //        });
    //    });
    //}
    function GetCDRData() {
      
        return CDRAPIService.GetCDRData($scope.fromDate, $scope.toDate, $scope.selectedsize, $scope.selectedCDROption).then(function (response) {
            //  alert(response);
            $scope.data = [];
            console.log(response);
            $scope.isInitializing = false;
            angular.forEach(response, function (itm) {
                $scope.data.push(itm);
            });

        });
    }
   

};

appControllers.controller('Analytics_CDRLogController', CDRLogController);