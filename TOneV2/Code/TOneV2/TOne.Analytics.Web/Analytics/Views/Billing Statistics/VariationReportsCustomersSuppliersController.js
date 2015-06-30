VariationReportsCustomersSuppliersController.$inject = ['$scope', 'BillingStatisticsAPIService', 'VariationReportOptionsEnum', 'EntityTypeEnum'];
function VariationReportsCustomersSuppliersController($scope, BillingStatisticsAPIService, VariationReportOptionsEnum, EntityTypeEnum) {
    var fromDate;
    var periodCount;
    var timePeriod;
    var reportOption;

    defineScope();
    load();

    function defineScope() {
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
    }

    function getData() {
        console.log(reportOption);
        $scope.isGettingData = true;
        return BillingStatisticsAPIService.GetVariationReport(fromDate, periodCount, timePeriod, reportOption, 0, 10, EntityTypeEnum.Zone.value, $scope.dataItem.ID).then(function (response) {
            $scope.timeRanges.length = 0;
            $scope.data.length = 0;
            $scope.totalData.length = 0;
            $scope.TotalValues.length = 0;
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

appControllers.controller('VariationReportsCustomersSuppliersController', VariationReportsCustomersSuppliersController);
