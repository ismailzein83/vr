VariationReportsController.$inject = ['$scope', 'BillingStatisticsAPIService', 'TimePeriodEnum', 'VariationReportOptionsEnum'];

function VariationReportsController($scope, BillingStatisticsAPIService, TimePeriodEnum, VariationReportOptionsEnum) {
    var timePeriod = [];
    var variationReportOptions = [];
    $scope.name = "test";
    $scope.isInitializing = false;
    $scope.fromDate = '2013/07/31';
    $scope.toDate = '2015/05/01';
    $scope.periodTypeValue = 'Days';
    $scope.selectedReportOption = 'InBoundMinutes';
    $scope.periodCount = 7;
    $scope.data = [];
    defineScope();
    loadTimePeriods();
    loadVariationReportOptions();

    function defineScope() {
        $scope.getData = function () {
            return getData(true);
        };
        $scope.getVariationReportsData = getVariationReportsData;
        $scope.timePeriod = timePeriod;
        $scope.variationReportOptions = variationReportOptions;
        $scope.periodValuesArray = [];
    }
    
    function loadTimePeriods() {
        for (var prop in TimePeriodEnum) {
            timePeriod.push(TimePeriodEnum[prop].description);
        }
    }
    function loadVariationReportOptions() {
        for (var prop in VariationReportOptionsEnum) {
        variationReportOptions.push(VariationReportOptionsEnum[prop].description);
        }
    }
    function getVariationReportsData() {
        $scope.isInitializing = true;
        $scope.data = [];
        BillingStatisticsAPIService.GetVariationReport($scope.fromDate, $scope.periodCount, TimePeriodEnum[$scope.periodTypeValue].description, VariationReportOptionsEnum[$scope.selectedReportOption].description).then(function (response) {
            $scope.isInitializing = false;
            console.log(response);
            if (response.length > 0) {
                $scope.periodValuesArray.length = 0;
                angular.forEach(response[0].TotalDurationsPerDate, function (item) {
                    $scope.periodValuesArray.push(item.CallDate);
                });
            }
            angular.forEach(response, function (itm) { $scope.data.push(itm); });
        });
    }
};


appControllers.controller('Analytics_VariationReportsController', VariationReportsController);