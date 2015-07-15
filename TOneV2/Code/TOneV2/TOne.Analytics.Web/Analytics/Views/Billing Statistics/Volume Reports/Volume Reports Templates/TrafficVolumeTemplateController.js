TrafficVolumeTemplateController.$inject = ['$scope', 'BillingStatisticsAPIService', 'VolumeReportsTimePeriodEnum', 'VolumeReportsOptionsEnum', 'CarrierTypeEnum', 'CarrierAPIService', 'ZonesService'];
function TrafficVolumeTemplateController($scope, BillingStatisticsAPIService, VolumeReportsTimePeriodEnum, VolumeReportsOptionsEnum, CarrierTypeEnum, CarrierAPIService, ZonesService) {

    var fromDate;
    var toDate;
    var selectedTrafficReport;
    var timePeriod;
    var customerId;
    var supplierId;
    var zoneId;
    var attempts;
    var chartAPI;
   

    defineScope();
    load();


    function defineScope() {
        $scope.chartData = [];
        $scope.chartReady = function (api) {
            chartAPI = api;
        };
    }

    function load() {
        loadFilters();
        loadTrafficVolumeData();
    }
    function loadFilters() {
        fromDate = $scope.filter.fromDate;
        toDate =  $scope.filter.toDate;
        selectedTrafficReport = $scope.filter.selectedTrafficReport;
        $scope.selectedTrafficReport = selectedTrafficReport;
        timePeriod = $scope.filter.timePeriod;
        customerId = $scope.filter.customerId;
        supplierId = $scope.filter.supplierId; 
        zoneId = $scope.filter.zoneId;
        attempts = $scope.filter.attempts;
    }


    function loadTrafficVolumeData() {
        $scope.isLoading = true;
        return BillingStatisticsAPIService.GetVolumeReportData(fromDate, toDate, customerId, supplierId, zoneId, attempts, timePeriod.description, selectedTrafficReport.value).then(function (response) {
            $scope.chartData.length = 0;
            angular.forEach(response, function (item) {
                var attempts = item.Attempts;
                var duration = item.Duration;
                $scope.chartData.push(item);
            });
            updateChart($scope.chartData);

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
appControllers.controller('Analytics_TrafficVolumeTemplateController', TrafficVolumeTemplateController);
