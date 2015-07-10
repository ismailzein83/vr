VolumeReportsController.$inject = ['$scope', 'VolumeReportsAPIService', 'VolumeReportsTimePeriodEnum', 'VolumeReportsOptionsEnum', 'CarrierTypeEnum', 'CarrierAPIService', 'ZonesService'];
function VolumeReportsController($scope, VolumeReportsAPIService, VolumeReportsTimePeriodEnum, VolumeReportsOptionsEnum, CarrierTypeEnum, CarrierAPIService, ZonesService) {

    var chartAPI;
    defineScope();
    load();


    function defineScope() {
        $scope.fromDate = '2013/07/01';
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
        console.log(filter.CustomerIds);
        console.log(filter.SupplierIds);
        console.log(filter.ZoneIds);
        return VolumeReportsAPIService.GetVolumeReportData($scope.fromDate, $scope.toDate, filter.CustomerIds, filter.SupplierIds, filter.ZoneIds, $scope.attempts, $scope.selectedTimePeriod.description).then(function (response) {
            angular.forEach(response, function (item) {
                var attempts = item.Attempts;
                var Duration = item.Duration;
                $scope.chartData.push(item);
                console.log($scope.chartData);
              });
            console.log(response);
            updateChart( $scope.chartData);

        }).finally(function () {
            $scope.isLoading = false;
        });
    }


    function updateChart(chartData) {

        //if ($scope.chartSaleCostProfitAPI == undefined)
        //    return;

        //if ($scope.chartProfitAPI == undefined)
        //    return;
        var fromDate = $scope.fromDate;
        var toDate = $scope.toDate;
        if (fromDate == undefined || toDate == undefined)
            return;

      //  $scope.chartData.length = 0;
        $scope.isGettingData = true;
       // var selectedTimeDimension = $scope.timeDimensionTypesOption.lastselectedvalue;
       
        var data = chartData;
        var chartDefinition = {
                    type: "column",
                    //    title: "Cost/Sale/Profit",
                    yAxisTitle: "Value"
        };
        var xAxisDefinition = { titlePath: "xValue" };
         //       var xAxisDefinition = { titlePath: "dateTimeValue", groupNamePath: "dateTimeGroupValue" };
                //var seriesDefinitions = [{
                //    title: "PROFIT",
                //    valuePath: "Values[2]",
                //    type: "spline"
                //}
        //];
        var seriesDefinitions = [];
        for (var i = 0; i < data.length; i++) {
            var dataItem = data[i];
            seriesDefinitions.push({
                title: dataItem.Name,
                valuePath: "Values[" + i + "]",
                type: "column"
            });

            for (var j = 1; j < data.length + 1; j++) {
                chartData.Values[i] = dataItem.Values[j - 1];
            }
        }

        
                chartAPI.renderChart(chartData, chartDefinition, seriesDefinitions, xAxisDefinition);
            
                $scope.isGettingData = false;
            
    }
   
};

appControllers.controller('Analytics_VolumeReportsController', VolumeReportsController);