VolumeReportsController.$inject = ['$scope', 'BillingStatisticsAPIService', 'VolumeReportsTimePeriodEnum', 'VolumeReportsOptionsEnum', 'CarrierTypeEnum', 'CarrierAPIService', 'ZonesService'];
function VolumeReportsController($scope, BillingStatisticsAPIService, VolumeReportsTimePeriodEnum, VolumeReportsOptionsEnum, CarrierTypeEnum, CarrierAPIService, ZonesService) {

    var chartAPI;
    defineScope();
    load();


    function defineScope() {
        $scope.fromDate = '2012/07/01';
        $scope.toDate = '2015/07/10';
        $scope.timePeriods = [];
        $scope.trafficTypeReports = [];
        $scope.customers = [];
        $scope.selectedCustomers = [];
        $scope.suppliers = [];
        $scope.selectedSuppliers = [];
        $scope.selectedZones = [];
        $scope.searchZones = function (text) {
            return ZonesService.getSalesZones(text);
        }
        loadCustomers();
        loadSuppliers();
        loadTimePeriods();
        loadVolumeReportsOptions();
        

        $scope.data = [];
        $scope.chartData = [];
        $scope.filterObject = [];

        $scope.onSearch = function () {
             return getVolumeReportsData();         
        }
        $scope.chartReady = function (api) {
            chartAPI = api;
        };
    }
    function buildFilter() {
        var filter = {};
        filter.CustomerIds = getFilterIds($scope.selectedCustomers, "CarrierAccountID");
        filter.SupplierIds = getFilterIds($scope.selectedSuppliers, "CarrierAccountID");
        filter.ZoneIds = getFilterIds($scope.selectedZones, "ZoneId");
        return filter;
    }

    function getFilterIds(values, idProp) {
        var filterIds = [];
        if (values.length > 0) {
            angular.forEach(values, function (val) {
                filterIds.push(val[idProp]);
            });
        }
        return filterIds;
    }

    function load() { }
    function loadTimePeriods() {
        $scope.timePeriods = [];
        for (var prop in VolumeReportsTimePeriodEnum) {
            $scope.timePeriods.push(VolumeReportsTimePeriodEnum[prop]);
        }
        $scope.selectedTimePeriod = $scope.timePeriods[0];
    }
    function loadVolumeReportsOptions() {
        $scope.trafficTypeReports = [];
        for (var prop in VolumeReportsOptionsEnum) {
            $scope.trafficTypeReports.push(VolumeReportsOptionsEnum[prop]);
        }
        $scope.selectedReportOption = $scope.trafficTypeReports[0];
    }
    function loadCustomers() {
        return CarrierAPIService.GetCarriers(CarrierTypeEnum.Customer.value).then(function (response) {
            angular.forEach(response, function (itm) {
                $scope.customers.push(itm);
            });
        });
    }
    function loadSuppliers() {
        return CarrierAPIService.GetCarriers(CarrierTypeEnum.Supplier.value).then(function (response) {
            angular.forEach(response, function (itm) {
                $scope.suppliers.push(itm);
            });
        });
    }
    function getVolumeReportsData() {
         $scope.isLoading = true;
        var filter = buildFilter();
       // console.log(filter.CustomerIds);
      //  console.log(filter.SupplierIds);
        //  console.log(filter.ZoneIds);
      
        return VolumeReportsAPIService.GetVolumeReportData($scope.fromDate, $scope.toDate, filter.CustomerIds, filter.SupplierIds, filter.ZoneIds, $scope.attempts, $scope.selectedTimePeriod.description, $scope.selectedTrafficReport.value).then(function (response) {
            $scope.chartData.length = 0;
            angular.forEach(response, function (item) {
                var attempts = item.Attempts;
                var duration = item.Duration;
                $scope.chartData.push(item);       
              });      
            updateChart( $scope.chartData);

        }).finally(function () {
            $scope.isLoading = false;
        });
    }


    function updateChart(chartData) {
        var chartDefinition = {
            type: "column",
            title: "Traffic volume"
        };
        var xAxisDefinition = {
            titlePath: "Values"
        };
        var seriesDefinitions = [];
        var seriesDefinitions = [{
            title: "Attempts",
            valuePath: "Values[0]",
            type: "column"
        }, {
            title: "Durations",
            valuePath: "Values[1]",
            type: "column"
        }
        ];

        console.log(seriesDefinitions);
        chartAPI.renderChart(chartData, chartDefinition, seriesDefinitions, xAxisDefinition);
     

            
    }
   
};

appControllers.controller('Analytics_VolumeReportsController', VolumeReportsController);