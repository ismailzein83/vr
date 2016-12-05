VariationReportsCustomersSuppliersController.$inject = ['$scope', 'BillingStatisticsAPIService', 'VariationReportOptionsEnum', 'EntityTypeEnum','GroupingByEnum'];
function VariationReportsCustomersSuppliersController($scope, BillingStatisticsAPIService, VariationReportOptionsEnum, EntityTypeEnum, GroupingByEnum) {

    var fromDate;
    var periodCount;
    var timePeriod;
    var reportOption;
    var mainGridCustomersAPI;
    var mainGridSuppliersAPI;

    defineScope();
    load();

    function defineScope() {
        $scope.customersData = [];
        $scope.suppliersData = [];
        $scope.totalData = [];
        $scope.TotalValues = [];
        $scope.periodValuesArray = [];
        $scope.onMainCustomersGridReady = function (api) {
            mainGridCustomersAPI = api;
        }
        $scope.onMainSuppliersGridReady = function (api) {
            mainGridSuppliersAPI=api
        }
        loadFilters();
        getCustomersData();
        getSuppliersData();
    }
    function load() {

    }
    function loadFilters() {
        fromDate = $scope.viewScope.filterObject.SelectedDate;
        periodCount = $scope.viewScope.filterObject.PeriodCount;
        timePeriod = $scope.viewScope.filterObject.TimePeriod;
        $scope.timePeriod = timePeriod;
        reportOption = $scope.viewScope.filterObject.ReportOption;
        $scope.timeRanges = $scope.viewScope.timeRanges;
        $scope.ZoneName = $scope.dataItem.Name;
        var entityType;
        switch (reportOption) {
            case VariationReportOptionsEnum.TopDestinationMinutes:
                reportOption = VariationReportOptionsEnum.InBoundMinutes;
                entityType = EntityTypeEnum.Zone;
                break;
            case VariationReportOptionsEnum.TopDestinationAmount:
                reportOption = VariationReportOptionsEnum.InBoundAmount;
                entityType = EntityTypeEnum.Zone;
                break;
        }
    }

    function getCustomersData() {
        $scope.isGettingData = true;
        switch (reportOption) {

            case VariationReportOptionsEnum.InOutBoundMinutes:
                reportOption = VariationReportOptionsEnum.InBoundMinutes;
                var name = $scope.dataItem.Name;
                if (name.match("/IN$")) {
                    entityType = EntityTypeEnum.Customer;
               }
                else if (name.match("/OUT$")) {
                    entityType = EntityTypeEnum.Supplier;
                }
                break;
            case VariationReportOptionsEnum.InOutBoundAmount:
                reportOption = VariationReportOptionsEnum.InBoundAmount;
                var name = $scope.dataItem.Name;
                if (name.match("/IN$")) {
                    entityType = EntityTypeEnum.Customer;
                }
                else if (name.match("/OUT$")) {
                    entityType = EntityTypeEnum.Supplier;
                }
                break;
           
        }

        BillingStatisticsAPIService.GetVariationReport(fromDate, periodCount, timePeriod.value, reportOption.value, 0, 10, EntityTypeEnum.Zone.value, $scope.dataItem.ID, GroupingByEnum.Customers.value).then(function (response) {
            $scope.customersData.length = 0;
            $scope.totalData.length = 0;
            $scope.TotalValues.length = 0;
            setTimeout(function () {
                $scope.$apply(function () {
                    angular.forEach(response.VariationReportsData, function (item) { $scope.customersData.push(item); $scope.periodValuesArray.push(item.Values); });
                    if (response.TotalValues != null) {
                        $scope.summarydata = response;
                        $scope.TotalValues = response.TotalValues;
                        mainGridCustomersAPI.setSummary($scope.summarydata);
                    }
                });
            }, 1);
        }).finally(function () {
            $scope.isGettingData = false;
        });
    }

    function getSuppliersData() {
        $scope.isGettingData = true;

        BillingStatisticsAPIService.GetVariationReport(fromDate, periodCount, timePeriod.value, reportOption.value, 0, 10, EntityTypeEnum.Zone.value, $scope.dataItem.ID, GroupingByEnum.Suppliers.value).then(function (secondresponse) {
            $scope.suppliersData.length = 0;
            $scope.totalData.length = 0;
            $scope.TotalValues.length = 0;
            setTimeout(function () {
                $scope.$apply(function () {
                    angular.forEach(secondresponse.VariationReportsData, function (item) { $scope.suppliersData.push(item); $scope.periodValuesArray.push(item.Values); });
                    if (secondresponse.TotalValues != null) {
                        $scope.summarydata = secondresponse;
                        $scope.TotalValues = secondresponse.TotalValues;
                        mainGridSuppliersAPI.setSummary($scope.summarydata);
                    }
                });
            }, 1);
        }).finally(function () {
            $scope.isGettingData = false;   
        });
    }
};

appControllers.controller('VariationReportsCustomersSuppliersController', VariationReportsCustomersSuppliersController);
