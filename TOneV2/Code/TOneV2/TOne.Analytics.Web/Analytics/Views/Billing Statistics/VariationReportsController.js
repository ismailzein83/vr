VariationReportsController.$inject = ['$scope', 'BillingStatisticsAPIService', 'TimePeriodEnum', 'VariationReportOptionsEnum'];

function VariationReportsController($scope, BillingStatisticsAPIService, TimePeriodEnum, VariationReportOptionsEnum) {

    var chartAPI;
    defineScope();


    function defineScope() {
        $scope.fromDate = '2013/07/31';
        $scope.periodCount = 7;
        $scope.data = [];
        $scope.timeRanges = [];
        $scope.chartData = [];

        loadTimePeriods();
        loadVariationReportOptions();

        $scope.periodValuesArray = [];
        $scope.mainGridPagerSettings = {
            currentPage: 1,
            totalDataCount: 0,
            pageChanged: function () {
                return getVariationReportsData();
            }
        };
        $scope.onSearch = function () {
            $scope.mainGridPagerSettings.currentPage = 1;
            return getVariationReportsData();
        }
        $scope.chartReady = function (api) {
            chartAPI = api;
        };
    }

    function loadTimePeriods() {
        $scope.timePeriods = [];
        for (var prop in TimePeriodEnum) {
            $scope.timePeriods.push(TimePeriodEnum[prop]);
        }
        $scope.selectedTimePeriod = $scope.timePeriods[0];
    }

    function loadVariationReportOptions() {
        $scope.variationReportOptions = [];
        for (var prop in VariationReportOptionsEnum) {
            $scope.variationReportOptions.push(VariationReportOptionsEnum[prop]);
        }
        $scope.selectedReportOption = $scope.variationReportOptions[0];
    }

    function getVariationReportsData() {
        $scope.isLoading = true;
        var pageInfo = $scope.mainGridPagerSettings.getPageInfo();
        return BillingStatisticsAPIService.GetVariationReport($scope.fromDate, $scope.periodCount, $scope.selectedTimePeriod.value, $scope.selectedReportOption.value, pageInfo.fromRow, pageInfo.toRow).then(function (response) {
            $scope.timeRanges.length = 0;
            $scope.data.length = 0;
            $scope.mainGridPagerSettings.totalDataCount = response.TotalCount;

            angular.forEach(response.TimeRange, function (item) {
                var fromDate = new Date(item.FromDate);
                var toDate = new Date(item.ToDate);
                if ($scope.selectedTimePeriod == TimePeriodEnum.Days)
                    $scope.timeRanges.push(fromDate.getDate() + "-" + (fromDate.getMonth() + 1) + "-" + (fromDate.getFullYear() ));
                else
                    $scope.timeRanges.push(fromDate.getDate() + "-" + (fromDate.getMonth() + 1) + "-" + (fromDate.getFullYear()) + "/" + (toDate.getDate() + "-" + (toDate.getMonth() + 1) + "-" + (fromDate.getFullYear() )));
            });
            setTimeout(function () {
                $scope.$apply(function () {
                    angular.forEach(response.VariationReportsData, function (item) { $scope.data.push(item); $scope.periodValuesArray.push(item.Values); });
                });
            }, 1);
            updateChart($scope.timeRanges, response.VariationReportsData);

        }).finally(function () {
            $scope.isLoading = false;

        });
    }

    function updateChart(times, data) {

        var chartDefinition = {
            type: "column",
            yAxisTitle: "Value"
        };
        var xAxisDefinition = { titlePath: "xValue" };
        var chartData = [];
        var seriesDefinitions = [];
       
        chartData.push({ xValue: "AVG", Values: [] });
        for (var j = 0; j < data.length ; j++) {
            chartData[0].Values[j] = data[j].PeriodTypeValueAverage;
        }
       
        angular.forEach(times, function (itm) {
            chartData.push({
                xValue: itm,
                Values: []
            });
        });
       
        for (var i = 0; i < data.length; i++) {
            var dataItem = data[i];
            seriesDefinitions.push({
                title: dataItem.Name,
                valuePath: "Values[" + i + "]",
                type: "column"
            });
          
            for (var j = 1; j < times.length+1; j++) {
               chartData[j].Values[i] = dataItem.Values[j-1];
            }
        }
        chartAPI.renderChart(chartData, chartDefinition, seriesDefinitions, xAxisDefinition);
    }

};


appControllers.controller('Analytics_VariationReportsController', VariationReportsController);