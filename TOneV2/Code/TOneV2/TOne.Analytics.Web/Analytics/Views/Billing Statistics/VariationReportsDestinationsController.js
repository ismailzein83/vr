VariationReportsDestinationsController.$inject = ['$scope', 'BillingStatisticsAPIService', 'VariationReportOptionsEnum', 'EntityTypeEnum', 'GroupingByEnum'];
function VariationReportsDestinationsController($scope, BillingStatisticsAPIService, VariationReportOptionsEnum,EntityTypeEnum,GroupingByEnum) {
   
    var fromDate;
    var periodCount;
    var timePeriod;
    var reportOption;
    var mainGridAPI;

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
        $scope.showCustomersSuppliersSection = true;
        $scope.onMainGridReady = function (api) {
            mainGridAPI = api;
        }
        loadFilters();
        getData();
      

    }
    function load() {
      
    }
    function loadFilters() {
        fromDate = $scope.viewScope.filterObject.SelectedDate;
        periodCount = $scope.viewScope.filterObject.PeriodCount;
        timePeriod = $scope.viewScope.filterObject.TimePeriod;
        $scope.timePeriod = timePeriod;
        reportOption = $scope.viewScope.filterObject.ReportOption;

        //  console.log($scope.viewScope.filterObject.SelectedDate);
        //  fromDate = getPropValuesFromArray($scope.viewScope.filterObject, 'SelectedDate');
    }

    function getData() {

        $scope.isGettingData = true; 
       // console.log($scope.dataItem);
        // console.log($scope.dataItem.ID);
        console.log($scope.dataItem.Name);
        
        console.log(reportOption);
        var selectedReportOption = reportOption;//$scope.viewScope.selectedReportOption;
      //  $scope.showCustomersSuppliersSection = selectedReportOption == VariationReportOptionsEnum.InBoundMinutes;
   //     console.log(selectedReportOption);
        var entityType;
        switch (selectedReportOption) {
            case VariationReportOptionsEnum.InBoundMinutes:
                selectedReportOption = VariationReportOptionsEnum.TopDestinationMinutes;
                entityType = EntityTypeEnum.Customer;
                $scope.showCustomersSuppliersSection = true;
                break;
            case VariationReportOptionsEnum.OutBoundMinutes:
                selectedReportOption = VariationReportOptionsEnum.TopDestinationMinutes;
                entityType = EntityTypeEnum.Supplier;
                $scope.showCustomersSuppliersSection = false;
                break;
            case VariationReportOptionsEnum.InOutBoundMinutes:
                selectedReportOption = VariationReportOptionsEnum.TopDestinationMinutes;
                //  entityType = EntityTypeEnum.Customers;
                //$scope.show = true;
                break;
            case VariationReportOptionsEnum.InBoundAmount:
                selectedReportOption = VariationReportOptionsEnum.TopDestinationAmount
                entityType = EntityTypeEnum.Customer;
                $scope.showCustomersSuppliersSection = true;
                break;
            case VariationReportOptionsEnum.OutBoundAmount:
                selectedReportOption = VariationReportOptionsEnum.TopDestinationAmount
                entityType = EntityTypeEnum.Supplier;
                $scope.showCustomersSuppliersSection = false;
                break;
            case VariationReportOptionsEnum.InOutBoundAmount:
                selectedReportOption = VariationReportOptionsEnum.TopDestinationAmount
                //  entityType = EntityTypeEnum.Customers;
                // $scope.show = true;
                break;
            case VariationReportOptionsEnum.Profit:
                selectedReportOption = VariationReportOptionsEnum.TopDestinationAmount
                entityType = EntityTypeEnum.Customer;
                $scope.showCustomersSuppliersSection = true;
                break;           
        }
        $scope.selectedReportOption = selectedReportOption;
        $scope.entityType = entityType;
      //  console.log(selectedReportOption);
        // console.log(entityType);
        console.log($scope.showCustomersSuppliersSection);
        return BillingStatisticsAPIService.GetVariationReport(fromDate, periodCount, timePeriod.value, selectedReportOption.value, 0, 10, entityType.value, $scope.dataItem.ID, GroupingByEnum.none.value).then(function (response) {
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
                    mainGridAPI.setSummary($scope.summarydata);
                });
            }, 1);  
        }).finally(function () {
            $scope.isGettingData = false;
       //     console.log($scope.data);
        });
    }
      
};

appControllers.controller('VariationReportsDestinationsController', VariationReportsDestinationsController);
