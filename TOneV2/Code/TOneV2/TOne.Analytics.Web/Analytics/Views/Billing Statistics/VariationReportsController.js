VariationReportsController.$inject = ['$scope', 'BillingStatisticsAPIService', 'TimePeriodEnum', 'VariationReportOptionsEnum'];

function VariationReportsController($scope, BillingStatisticsAPIService, TimePeriodEnum, VariationReportOptionsEnum) {
    var timePeriod = [];
    var variationReportOptions = [];
    $scope.isInitializing = false;
    $scope.fromDate = '2013/07/31';
    $scope.periodTypeValue = 'Days';
    $scope.selectedReportOption = 'InBoundMinutes';
    $scope.periodCount = 7;
    $scope.data = [];
    $scope.timeRanges = [];
    defineScope();
    loadTimePeriods();
    loadVariationReportOptions();

    function defineScope() {
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
        BillingStatisticsAPIService.GetVariationReport($scope.fromDate, $scope.periodCount, TimePeriodEnum[$scope.periodTypeValue].description, VariationReportOptionsEnum[$scope.selectedReportOption].description).then(function (response) {
           
            console.log(response);
            //if (response.length > 0) {
            //    $scope.periodValuesArray.length = 0;
            //    angular.forEach(response[0].TotalDurationsPerDate, function (item) {
            //        $scope.periodValuesArray.push(item.CallDate);
            //    });
            //}   $scope.periodValuesArray.length = 0;          
            // $scope.periodValuesArray.push(itm.VariationReportsData.TotalDurationsPerDate);

            $scope.timeRanges.length = 0;
            $scope.data.length = 0;
            angular.forEach(response.VariationReportsData, function (item) { $scope.data.push(item); $scope.periodValuesArray.push(item.TotalDurationsPerDate); });
            angular.forEach(response.TimeRange, function (item) {
                if (TimePeriodEnum[$scope.periodTypeValue].description == "Days")
                    $scope.timeRanges.push(new Date(item.FromDate).toDateString());
                else {
                    var toDate = new Date(item.ToDate);
                    toDate.setDate(toDate.getDate() - 1);
                    $scope.timeRanges.push(new Date(item.FromDate).toDateString() + "/" + toDate.toDateString());
                }
            });

        }).finally(function () {
             $scope.isInitializing = false;
        });
    }
};


appControllers.controller('Analytics_VariationReportsController', VariationReportsController);