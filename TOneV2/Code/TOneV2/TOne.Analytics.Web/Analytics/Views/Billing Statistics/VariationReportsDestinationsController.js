﻿VariationReportsDestinationsController.$inject = ['$scope', 'BillingStatisticsAPIService', 'VariationReportOptionsEnum', 'EntityTypeEnum'];
function VariationReportsDestinationsController($scope, BillingStatisticsAPIService, VariationReportOptionsEnum,EntityTypeEnum) {
   
    var fromDate;
    var periodCount;
    var timePeriod;
    var reportOption;

    defineScope();
    load();


    function defineScope() {
        //  $scope.fromDate = '2013/07/31';
        //  $scope.periodCount = 7;
        $scope.data = [];
        $scope.totalData = [];
        $scope.timeRanges = [];
        $scope.TotalValues = [];
        $scope.periodValuesArray = [];
        loadFilters();
        getData();


    }
    function load() {
      
    }
    function loadFilters() {
        fromDate = $scope.viewScope.filterObject.SelectedDate;
        periodCount = $scope.viewScope.filterObject.PeriodCount;
        timePeriod = $scope.viewScope.filterObject.TimePeriod;
        reportOption = $scope.viewScope.filterObject.ReportOption;
        
        //  console.log($scope.viewScope.filterObject.SelectedDate);
        //  fromDate = getPropValuesFromArray($scope.viewScope.filterObject, 'SelectedDate');
    }

    function getData() {

        $scope.isGettingData = true;
       // console.log($scope.dataItem);
        // console.log($scope.dataItem.ID);   
        var selectedReportOption = $scope.viewScope.selectedReportOption;
        switch (selectedReportOption) {
            case VariationReportOptionsEnum.InBoundMinutes:
                selectedReportOption = VariationReportOptionsEnum.TopDestinationMinutes;
                break;
            case VariationReportOptionsEnum.OutBoundMinutes:
                selectedReportOption = VariationReportOptionsEnum.TopDestinationMinutes;
                break;
            case VariationReportOptionsEnum.InOutBoundMinutes:
                selectedReportOption = VariationReportOptionsEnum.TopDestinationMinutes;
                break;  
            case VariationReportOptionsEnum.InBoundAmount:
                selectedReportOption = VariationReportOptionsEnum.TopDestinationAmount
                break;
            case VariationReportOptionsEnum.OutBoundAmount:
                selectedReportOption = VariationReportOptionsEnum.TopDestinationAmount
                break;
            case VariationReportOptionsEnum.InOutBoundAmount:
                selectedReportOption = VariationReportOptionsEnum.TopDestinationAmount
                break;
            case VariationReportOptionsEnum.Profit:
                selectedReportOption = VariationReportOptionsEnum.TopDestinationAmount
                break;               
        }
        $scope.selectedReportOption = selectedReportOption;

        return BillingStatisticsAPIService.GetVariationReport(fromDate, periodCount, timePeriod, selectedReportOption.value, 0, 10, EntityTypeEnum.Zone.value, $scope.dataItem.ID).then(function (response) {
            $scope.timeRanges.length = 0;
            $scope.data.length = 0;
            $scope.totalData.length = 0;
            $scope.TotalValues.length = 0;  
           // console.log($scope.viewScope.fromDate, $scope.viewScope.periodCount);
            $scope.timeRanges = $scope.viewScope.timeRanges;
            setTimeout(function () {
                $scope.$apply(function () {
                    angular.forEach(response.VariationReportsData, function (item) { $scope.data.push(item); $scope.periodValuesArray.push(item.Values); });
                    $scope.summarydata = response;
                    $scope.TotalValues = response.TotalValues;
                });
            }, 1);  
        }).finally(function () {
            $scope.isGettingData = false;
       //     console.log($scope.data);
        });
    }
      
};

appControllers.controller('VariationReportsDestinationsController', VariationReportsDestinationsController);
