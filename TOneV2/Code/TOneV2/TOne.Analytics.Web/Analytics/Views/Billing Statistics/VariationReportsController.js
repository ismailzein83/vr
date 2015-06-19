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
        $scope.getZoneProfit = getZoneProfit;
        $scope.getBillingStats = getBillingStats;
        $scope.getVariationReportsData = getVariationReportsData;
        //$scope.getVariationReportsFinalData = getVariationReportsFinalData;
        $scope.timePeriod = timePeriod;
        $scope.variationReportOptions = variationReportOptions;
        $scope.periodValuesArray = [];
    }
    function getData(withSummary) {
         alert($scope.name);
        if ($scope.name == undefined)
            $scope.name = "test";
        $scope.isInitializing = true;
          BillingStatisticsAPIService.GetTest($scope.name).then(function (response) {
              alert(response);
              $scope.isInitializing = false;
          });
    }
    function getZoneProfit() {
        BillingStatisticsAPIService.GetZoneProfit($scope.fromDate,$scope.toDate).then(function (response) {
            //  alert(response);
            console.log(response);
            $scope.isInitializing = false;
            angular.forEach(response, function (itm) {
                $scope.data.push(itm);
            });          
        });
    }
    function getBillingStats() {
        $scope.isInitializing = true;
        BillingStatisticsAPIService.GetBillingStatistics($scope.fromDate, $scope.toDate).then(function (response) {
            console.log(response);
            $scope.isInitializing = false;
            angular.forEach(response, function (itm) { $scope.data.push(itm);});
        });
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

    //function getVariationReportQuery() {
    //    BillingStatisticsAPIService.GetVariationReport($scope.fromDate, $scope.periodCount, TimePeriodEnum[$scope.periodTypeValue].description, VariationReportOptionsEnum[$scope.selectedReportOption].description).then(function (response) {
    //        console.log(response);
    //        angular.forEach(response, function (itm) { $scope.data.push(itm); });
    //    });
    //}

};


appControllers.controller('Analytics_VariationReportsController', VariationReportsController);