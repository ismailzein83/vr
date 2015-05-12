/// <reference path="ZoneMonitorSettings.html" />
/// <reference path="ZoneMonitor.html" />
appControllers.controller('TrafficStatisticsGridController',
    function ZoneMonitorController($scope, AnalyticsAPIService, TrafficStatisticsMeasureEnum, TrafficStatisticGroupKeysEnum) {
        var measures = [];

        defineScopeObjects();
        defineScopeMethods();
        load();
        function defineScopeObjects() {
            $scope.measures = measures;
            $scope.groupKeys = [];
            $scope.menuActions = [{
                name: "CDRs"
            }];
        }

        function defineScopeMethods() {

            $scope.selectGroupKey = function (groupKey) {
                $scope.selectedGroupKey = groupKey;
                if (!groupKey.isDataLoaded)
                    getData();
            };

            $scope.onEntityClicked = function (dataItem) {
                var parentGroupKeys = $scope.viewScope.groupKeys;

                var selectedGroupKeyInParent = $.grep(parentGroupKeys, function (parentGrpKey) {
                    return parentGrpKey.groupKeyEnumValue == $scope.selectedGroupKey.groupKeyEnumValue;
                })[0];
                $scope.viewScope.selectEntity(selectedGroupKeyInParent, dataItem.GroupKeyValues[0].Id, dataItem.GroupKeyValues[0].Name)
            };
        }

        function load() {
            loadMeasures();
            loadGroupKeys();
            if ($scope.selectedGroupKey != undefined)
                getData();
        }

        function loadMeasures() {
            for (var prop in TrafficStatisticsMeasureEnum) {
                measures.push(TrafficStatisticsMeasureEnum[prop]);
            }
        }

        function loadGroupKeys() {

            addGroupKeyIfNotExistsInParent({
                title: "Zones",
                groupKeyEnumValue: TrafficStatisticGroupKeysEnum.OurZone.value,
                data: [],
                isDataLoaded: false,
                gridHeader: "Zone"
            });
            addGroupKeyIfNotExistsInParent({
                title: "Customers",
                groupKeyEnumValue: TrafficStatisticGroupKeysEnum.CustomerId.value,
                data: [],
                isDataLoaded: false,
                gridHeader: "Customer"
            });
            addGroupKeyIfNotExistsInParent({
                title: "Suppliers",
                groupKeyEnumValue: TrafficStatisticGroupKeysEnum.SupplierId.value,
                data: [],
                isDataLoaded: false,
                gridHeader: "Supplier"
            });

            if ($scope.groupKeys.length > 0)
                $scope.selectedGroupKey = $scope.groupKeys[0];
        }

        function addGroupKeyIfNotExistsInParent(groupKey) {
            var parentGroupKeys = $scope.viewScope.currentSearchCriteria.groupKeys;
            if ($.grep(parentGroupKeys, function (parentGrpKey) {
                return parentGrpKey.groupKeyEnumValue == groupKey.groupKeyEnumValue;
            }).length == 0)
                $scope.groupKeys.push(groupKey);
        }

        function getData(asyncHandle) {
            var withSummary = false;
            var fromRow = 1;
            var toRow = 100;
           
            var getTrafficStatisticSummaryInput = {
                TempTableKey: null,
                Filter: buildFilter(),
                WithSummary: withSummary,
                GroupKeys: [$scope.selectedGroupKey.groupKeyEnumValue],
                From: $scope.viewScope.fromDate,
                To: $scope.viewScope.toDate,
                FromRow: fromRow,
                ToRow: toRow,
                OrderBy: 2,
                IsDescending: true
            };
            var isSucceeded;
            $scope.isGettingData = true;
            AnalyticsAPIService.GetTrafficStatisticSummary(getTrafficStatisticSummaryInput).then(function (response) {

                angular.forEach(response.Data, function (itm) {
                    $scope.selectedGroupKey.data.push(itm);
                });
                $scope.selectedGroupKey.isDataLoaded = true;
            })
                .finally(function () {
                    if (asyncHandle)
                        asyncHandle.operationDone(isSucceeded);
                    $scope.isGettingData = false;
                });
        }

        function buildFilter() {
            var filter = {};
            var parentGroupKeys = $scope.viewScope.currentSearchCriteria.groupKeys;
            for(var i =0;i<parentGroupKeys.length;i++) {
                var groupKey = parentGroupKeys[i];
                switch(groupKey.groupKeyEnumValue)
                {
                    case TrafficStatisticGroupKeysEnum.OurZone.value:
                        filter.ZoneIds = [$scope.dataItem.GroupKeyValues[i].Id];
                        break;
                    case TrafficStatisticGroupKeysEnum.CustomerId.value: 
                        filter.CustomerIds = [$scope.dataItem.GroupKeyValues[i].Id];
                        break;
                    case TrafficStatisticGroupKeysEnum.SupplierId.value: 
                        filter.SupplierIds = [$scope.dataItem.GroupKeyValues[i].Id];
                        break;
                }
            }
            return filter;
        }


    });