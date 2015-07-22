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
        $scope.data = [];
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
        toDate = $scope.filter.toDate;
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
        $scope.data.length = 0;
        return BillingStatisticsAPIService.GetTrafficVolumes(fromDate, toDate, customerId, supplierId, zoneId, attempts, timePeriod.value).then(function (response) {
            $scope.data.length = 0;
            angular.forEach(response, function (item) {
                var attempts = item.Attempts;
                var duration = item.Duration;
                var date = item.Date;
                $scope.data.push(item);
            });
            updateChart($scope.data);

        }).finally(function () {
            $scope.isLoading = false;
        });
    }
    function updateChart(data) {
        $scope.chartData.length = 0;

        var chartDefinition = {
            type: "column",
            title: "Traffic volume"
        };
        var xAxisDefinition = {
            titlePath: "Values",
        };


        $scope.chartData.push({ Values: [] });
        $scope.chartData[0].Values[0] = data[0].Attempts;
        $scope.chartData[0].Values[1] = data[0].Duration;
        $scope.chartData[0].Values[2] = data[0].Date;


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
        //console.log($scope.chartData);
        //console.log(chartDefinition);
        //console.log(seriesDefinitions);
        //console.log(xAxisDefinition);
        chartAPI.renderChart($scope.chartData, chartDefinition, seriesDefinitions, xAxisDefinition);



    }


};
appControllers.controller('Analytics_TrafficVolumeTemplateController', TrafficVolumeTemplateController);
