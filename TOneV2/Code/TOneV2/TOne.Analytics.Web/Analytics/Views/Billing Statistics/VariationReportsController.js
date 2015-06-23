VariationReportsController.$inject = ['$scope', 'BillingStatisticsAPIService', 'TimePeriodEnum', 'VariationReportOptionsEnum'];

function VariationReportsController($scope, BillingStatisticsAPIService, TimePeriodEnum, VariationReportOptionsEnum) {
   
    defineScope();
    load();

    function defineScope() {
        $scope.fromDate = '2013/07/31';
        $scope.periodCount = 7;
        $scope.data = [];
        $scope.timeRanges = [];

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
    }

    function load() {
        
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
           
            //console.log(response);
            //if (response.length > 0) {
            //    $scope.periodValuesArray.length = 0;
            //    angular.forEach(response[0].TotalDurationsPerDate, function (item) {
            //        $scope.periodValuesArray.push(item.CallDate);
            //    });
            //}   $scope.periodValuesArray.length = 0;          
            // $scope.periodValuesArray.push(itm.VariationReportsData.TotalDurationsPerDate);

            $scope.timeRanges.length = 0;
            $scope.data.length = 0;
            $scope.mainGridPagerSettings.totalDataCount = response.TotalCount;
            angular.forEach(response.VariationReportsData, function (item) { $scope.data.push(item); $scope.periodValuesArray.push(item.TotalDurationsPerDate); });
            angular.forEach(response.TimeRange, function (item) {
                if ($scope.selectedTimePeriod == TimePeriodEnum.Days)
                    $scope.timeRanges.push(new Date(item.FromDate).toDateString());
                else {
                    var toDate = new Date(item.ToDate);
                    toDate.setDate(toDate.getDate() - 1);
                    $scope.timeRanges.push(new Date(item.FromDate).toDateString() + "/" + toDate.toDateString());
                }
            });

        }).finally(function () {
            $scope.isLoading = false;
        });
    }
};


appControllers.controller('Analytics_VariationReportsController', VariationReportsController);