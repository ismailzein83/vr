VariationReportsController.$inject = ['$scope', 'BillingStatisticsAPIService'];

function VariationReportsController($scope, BillingStatisticsAPIService) {

    var mainGridapi;
    var sortColumn;
    var sortDescending = true;

    $scope.data = [
        { name: 'Jani', country: 'Norway' },
        { name: 'Hege', country: 'Sweden' },
        { name: 'Kai', country: 'Denmark' }
    ];
    $scope.fromDate = '2015/05/28';

    //loadParameters();
    defineScope();
    load();

    //function loadParameters() { }
    function defineScope() {

        $scope.testModel = 'VariationReportsController';
        $scope.mainGridPagerSettings = {
            currentPage: 1,
            totalDataCount: 0,
            pageChanged: function () {
                return getData();
            }
        };
        $scope.onMainGridReady = function (api) {
            mainGridAPI = api;
        }
        $scope.getData = function () {
            alert('$scope.getData ');
            //$scope.mainGridPagerSettings.currentPage = 1;
            //resultKey = null;
            //mainGridAPI.resetSorting();
            //resetSorting();
            return getData(true);
        };
    }
    function load() { }
    function resetSorting() {
        sortColumn = TrafficStatisticsMeasureEnum.Attempts;
        sortDescending = true;
    }
    function getData(withSummary) {
        alert($scope.name);
        if ($scope.name == undefined)
            $scope.name = "test";
        BillingStatisticsAPIService.GetTest($scope.name).then(function (response) {
            alert(response);
        });
        //    if (!chartSelectedMeasureAPI)
        //        return;
        //    if (sortColumn == undefined)
        //        return;
        //    if (withSummary == undefined)
        //        withSummary = false;


        //    if (chartSelectedEntityAPI)
        //        chartSelectedEntityAPI.hideChart();

        //    var count = $scope.mainGridPagerSettings.itemsPerPage;
        //    var groupKeys = [];

        //    angular.forEach($scope.selectedGroupKeys, function (group) {
        //        groupKeys.push(group.groupKeyEnumValue);
        //    });
        //    var pageInfo = $scope.mainGridPagerSettings.getPageInfo();
        //    var filter = buildFilter();
        //    var getTrafficStatisticSummaryInput = {
        //        TempTableKey: resultKey,
        //        Filter: filter,
        //        WithSummary: withSummary,
        //        GroupKeys: groupKeys,
        //        From: $scope.fromDate,
        //        To: $scope.toDate,
        //        FromRow: pageInfo.fromRow,
        //        ToRow: pageInfo.toRow,
        //        OrderBy: sortColumn.value,
        //        IsDescending: sortDescending
        //    };
        //    var isSucceeded;
        //    $scope.showResult = true;
        //    $scope.isGettingData = true;
        //    $scope.data.length = 0;
        //    $scope.currentSearchCriteria.groupKeys.length = 0;
        //    angular.forEach($scope.selectedGroupKeys, function (group) {
        //        $scope.currentSearchCriteria.groupKeys.push(group);
        //    });

        //    return AnalyticsAPIService.GetTrafficStatisticSummary(getTrafficStatisticSummaryInput).then(function (response) {

        //        currentData = response.Data;
        //        if (currentSortedColDef != undefined)
        //            currentSortedColDef.currentSorting = sortDescending ? 'DESC' : 'ASC';

        //        resultKey = response.ResultKey;
        //        $scope.mainGridPagerSettings.totalDataCount = response.TotalCount;
        //        if (withSummary) {
        //            $scope.trafficStatisticSummary = response.Summary;
        //            $scope.overallData[0] = response.Summary;
        //        }
        //        angular.forEach(response.Data, function (itm) {
        //            $scope.data.push(itm);
        //        });
        //        renderOverallChart();
        //        isSucceeded = true;
        //    })
        //        .finally(function () {
        //            $scope.isGettingData = false;
        //        });
        //}
    }

};



appControllers.controller('Analytics_VariationReportsController', VariationReportsController);