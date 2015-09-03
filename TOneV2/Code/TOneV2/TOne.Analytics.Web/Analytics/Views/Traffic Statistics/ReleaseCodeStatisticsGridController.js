(function (appControllers) {

    "use strict";

    function releaseCodeStatisticsGridController($scope, analyticsService, analyticsApiService, trafficStatisticGroupKeysEnum) {

        var measures ;

        function retrieveData(groupKey, withSummary) {
            var filter = $scope.viewScope.filter;
            filter.build($scope);

            var query = {
                Filter: filter,
                WithSummary: withSummary,
                GroupKeys: [$scope.selectedGroupKey.value],
                From: $scope.viewScope.fromDate,
                To: $scope.viewScope.toDate
            };
            return groupKey.gridAPI.retrieveData(query);
        }

        function defineScope() {

            $scope.parentGroupKeys = [];
            $scope.measures = analyticsService.getReleaseCodeMeasureEnum();
            $scope.currentSearchCriteria = {
                groupKeys: []
            };
            $scope.groupKeys = [];
            $scope.menuActions = analyticsService.getSubGridMenuAction($scope);

            $scope.groupKeySelectionChanged = function () {

                if ($scope.selectedGroupKeyIndex != undefined) {
                    $scope.selectedGroupKey = $scope.groupKeys[$scope.selectedGroupKeyIndex];
                    if (!$scope.selectedGroupKey.isDataLoaded && $scope.selectedGroupKey.gridAPI != undefined) {
                        retrieveData($scope.selectedGroupKey, false);
                    }
                }
            };

            $scope.checkExpandablerow = function (groupKey) {
                return analyticsService.checkExpandableRow(groupKey,$scope.groupKeys);
            };
        }

        function addGroupKeyIfNotExistsInParent(groupKey) {
            var parentGroupKeys = $scope.viewScope.currentSearchCriteria.groupKeys;
            if ($.grep(parentGroupKeys, function (parentGrpKey) {
                return parentGrpKey.value === groupKey.value;
            }).length === 0) {
                groupKey.onGridReady = function (api) {
                    groupKey.gridAPI = api;
                    if ($scope.selectedGroupKey === groupKey)
                        retrieveData(groupKey, false);
                };
                groupKey.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {

                    return analyticsApiService.getReleaseCodeStatistics(dataRetrievalInput).then(function (response) {
                        $scope.selectedGroupKey.isDataLoaded = true;
                        onResponseReady(response);
                    });
                };
                $scope.groupKeys.push(groupKey);
            }
        }

        function loadParentGroupKeys(scope) {
            if (scope === $scope.viewScope) {
                for (var i = 0; i < scope.selectedGroupKeys.length; i++) {
                    $scope.parentGroupKeys.push(scope.selectedGroupKeys[i]);
                }
                return;
            }
            else {
                $scope.parentGroupKeys.push(scope.selectedGroupKey);
                loadParentGroupKeys(scope.gridParentScope);
            }

        }

        function loadGroupKeys() {
            for (var prop in trafficStatisticGroupKeysEnum) {
                if (trafficStatisticGroupKeysEnum.hasOwnProperty(prop)) {
                    var groupKey = {
                        title: trafficStatisticGroupKeysEnum[prop].title,
                        value: trafficStatisticGroupKeysEnum[prop].value,
                        data: [],
                        isDataLoaded: false,
                        gridHeader: trafficStatisticGroupKeysEnum[prop].gridHeader
                    };

                    addGroupKeyIfNotExistsInParent(groupKey);
                }
            }
            if ($scope.groupKeys.length > 0)
                $scope.selectedGroupKey = $scope.groupKeys[0];

            loadParentGroupKeys($scope.gridParentScope);

            analyticsService.applyGroupKeysRules($scope.parentGroupKeys, $scope.groupKeys);
        }

        function onLoad() {
            defineScope();
            measures = analyticsService.getReleaseCodeMeasureEnum();
            loadGroupKeys();
            $scope.selectedGroupKey = $scope.groupKeys[0];
        }

        onLoad();
    }

    releaseCodeStatisticsGridController.$inject = ['$scope', 'AnalyticsService', 'AnalyticsAPIService', 'TrafficStatisticGroupKeysEnum'];

    appControllers.controller('Analytics_ReleaseCodeStatisticsGridController', releaseCodeStatisticsGridController);

})(appControllers);