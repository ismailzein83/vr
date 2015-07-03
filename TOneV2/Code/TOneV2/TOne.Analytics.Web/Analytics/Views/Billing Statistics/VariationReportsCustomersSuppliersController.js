VariationReportsCustomersSuppliersController.$inject = ['$scope', 'BillingStatisticsAPIService', 'VariationReportOptionsEnum', 'EntityTypeEnum','GroupingByEnum'];
function VariationReportsCustomersSuppliersController($scope, BillingStatisticsAPIService, VariationReportOptionsEnum, EntityTypeEnum, GroupingByEnum) {

    var fromDate;
    var periodCount;
    var timePeriod;
    var reportOption;
    var mainGridAPI;

    defineScope();
    load();

    function defineScope() {
        $scope.customersData = [];
        $scope.suppliersData = [];
        $scope.totalData = [];
        $scope.timeRanges = [];
        $scope.TotalValues = [];
        $scope.periodValuesArray = [];
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
        reportOption = $scope.viewScope.filterObject.ReportOption;
    }

    function getData() {
        console.log(reportOption);
        $scope.isGettingData = true;
        console.log($scope.dataItem.ID);
        var selectedReportOption = reportOption;
        console.log(selectedReportOption);
        var entityType;
        switch (selectedReportOption) {
            case VariationReportOptionsEnum.TopDestinationMinutes:
                selectedReportOption = VariationReportOptionsEnum.InBoundMinutes;
                entityType = EntityTypeEnum.Zone;
                break;
            case VariationReportOptionsEnum.TopDestinationAmount:
                selectedReportOption = VariationReportOptionsEnum.InBoundAmount;
                entityType = EntityTypeEnum.Zone;
                break;
        }
        console.log(selectedReportOption);
        $scope.ZoneName = $scope.dataItem.Name;
        BillingStatisticsAPIService.GetVariationReport(fromDate, periodCount, timePeriod.value, selectedReportOption.value, 0, 10, EntityTypeEnum.Zone.value, $scope.dataItem.ID, GroupingByEnum.Customers.value).then(function (response) {
            $scope.timeRanges.length = 0;
            $scope.customersData.length = 0;
            $scope.totalData.length = 0;
            $scope.TotalValues.length = 0;
            $scope.timeRanges = $scope.viewScope.timeRanges;
            setTimeout(function () {
                $scope.$apply(function () {
                    angular.forEach(response.VariationReportsData, function (item) { $scope.customersData.push(item); $scope.periodValuesArray.push(item.Values); });
                    $scope.summarydata = response;
                    $scope.TotalValues = response.TotalValues;
                    mainGridAPI.setSummary($scope.summarydata);
                });
            }, 1);
        }).finally(function () {
            $scope.isGettingData = false;
            //     console.log($scope.data);
        });
        BillingStatisticsAPIService.GetVariationReport(fromDate, periodCount, timePeriod.value, selectedReportOption.value, 0, 10, EntityTypeEnum.Zone.value, $scope.dataItem.ID, GroupingByEnum.Suppliers.value).then(function (secondresponse) {
            $scope.timeRanges.length = 0;
            $scope.suppliersData.length = 0;
            $scope.totalData.length = 0;
            $scope.TotalValues.length = 0;
            $scope.timeRanges = $scope.viewScope.timeRanges;
            setTimeout(function () {
                $scope.$apply(function () {
                    angular.forEach(secondresponse.VariationReportsData, function (item) { $scope.suppliersData.push(item); $scope.periodValuesArray.push(item.Values); });
                    $scope.summarydata = secondresponse;
                    $scope.TotalValues = secondresponse.TotalValues;
                    mainGridAPI.setSummary($scope.summarydata);
                });
            }, 1);
        }).finally(function () {
            $scope.isGettingData = false;
             console.log($scope.data);
        });
    }




};

appControllers.controller('VariationReportsCustomersSuppliersController', VariationReportsCustomersSuppliersController);
