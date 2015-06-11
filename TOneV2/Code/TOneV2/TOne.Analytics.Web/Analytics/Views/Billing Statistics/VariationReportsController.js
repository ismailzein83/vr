VariationReportsController.$inject = ['$scope', 'BillingStatisticsAPIService', 'TimePeriodEnum', 'VariationReportOptionsEnum'];

function VariationReportsController($scope, BillingStatisticsAPIService, TimePeriodEnum, VariationReportOptionsEnum) {
    $scope.name = "test";
    $scope.isInitializing = false;
    
    var mainGridAPI;
    $scope.data = [];
    // $scope.TimePeriod = ['Days','Weeks','Months']
   // $scope.variationReportOptions = ['InBoundMinutes', 'OutBoundMinutes', 'InOutBoundMinutes', 'TopDestinationMinutes', 'InBoundAmount', 'OutBoundAmount', 'InOutBoundAmount', 'TopDestinationAmount', 'Profit' ]
    var timePeriod = [];
    var variationReportOptions = [];

    $scope.fromDate = '2013/07/31';
    $scope.toDate = '2015/05/01';
    $scope.periodTypeValue = 'Days';
    $scope.selectedReportOption = 'InBoundMinutes';
    $scope.periodCount = 7;
 
    //$scope.variationReportOption
    defineScope();
    loadTimePeriods();
    loadVariationReportOptions();
  
   
    function defineScope() {
       
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
        $scope.getZoneProfit = getZoneProfit;
        $scope.getBillingStats = getBillingStats;
        $scope.getVariationReportsData = getVariationReportsData;
        $scope.getVariationReportsFinalData = getVariationReportsFinalData;
        $scope.timePeriod = timePeriod;
        $scope.variationReportOptions = variationReportOptions;
        
    }
    function getData(withSummary) {
        // alert($scope.name);
        //if ($scope.name == undefined)
        //    $scope.name = "test";
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
    //function getVariationReportsData() {
    //    $scope.isInitializing = true;
    //    BillingStatisticsAPIService.GetVariationReportsData($scope.fromDate, $scope.periodCount, $scope.periodTypeValue).then(function (response) {
    //        $scope.isInitializing = false;
    //        angular.forEach(response, function (itm) { $scope.data.push(itm); });
    //    });
    //}

    function getVariationReportsData() {
            $scope.isInitializing = true;
            BillingStatisticsAPIService.GetVariationReportsData($scope.fromDate, $scope.periodCount, $scope.periodTypeValue, $scope.variationReportOptions.value).then(function (response) {
                $scope.isInitializing = false;
                angular.forEach(response, function (itm) { $scope.data.push(itm); });
            });
        }

    
    function getVariationReportsFinalData() {
        $scope.isInitializing = true;
        BillingStatisticsAPIService.GetVariationReportsFinalData($scope.fromDate, $scope.periodCount, $scope.periodTypeValue).then(function (response) {
        //    alert('here');
        //  console.log(response);
         $scope.isInitializing = false;
         angular.forEach(response, function (itm) { $scope.data.push(itm); });
            
        });
    }
    function loadTimePeriods() {
        for (var prop in TimePeriodEnum) {
            timePeriod.push(TimePeriodEnum[prop].propertyName);
        }
    }

    function loadVariationReportOptions() {
        for (var prop in VariationReportOptionsEnum) {
        variationReportOptions.push(VariationReportOptionsEnum[prop].propertyName);
        }
    }


};


appControllers.controller('Analytics_VariationReportsController', VariationReportsController);