(function (appControllers) {

    "use strict";

    CarrierSummaryGridController.$inject = ['$scope', 'AnalyticsAPIService', 'CarrierSummaryGroupKeysEnum', 'CarrierSummaryMeasureEnum', 'VRModalService', 'UtilsService', 'LabelColorsEnum', 'AnalyticsService'];
    function CarrierSummaryGridController($scope, AnalyticsAPIService, CarrierSummaryGroupKeysEnum, CarrierSummaryMeasureEnum, VRModalService, UtilsService, LabelColorsEnum, AnalyticsService) {
        var filter = {};
        var selectedGroupKeys = [];
        defineScope();
        load();
        function defineScope() {

            $scope.parentGroupKeys = [];
            $scope.measures = [];
            $scope.measuresGroupKeys = [];
            $scope.currentSearchCriteria = {
                groupKeys: []
            };
            $scope.groupKeys = [];

            $scope.onEntityClicked = function (dataItem) {
                var parentGroupKeys = $scope.viewScope.groupKeys;

                var selectedGroupKeyInParent = $.grep(parentGroupKeys, function (parentGrpKey) {
                    return parentGrpKey.value == $scope.selectedGroupKey.value;
                })[0];
                $scope.viewScope.selectEntity(selectedGroupKeyInParent, dataItem.GroupKeyValues[0].Id, dataItem.GroupKeyValues[0].Name)
            };

            $scope.groupKeySelectionChanged = function () {

                if ($scope.selectedGroupKeyIndex != undefined) {
                    $scope.selectedGroupKey = $scope.groupKeys[$scope.selectedGroupKeyIndex];
                    if (!$scope.selectedGroupKey.isDataLoaded && $scope.selectedGroupKey.gridAPI != undefined) {
                        // $scope.parentGroupKeys = [];
                        retrieveData($scope.selectedGroupKey, false);
                    }


                }
            };

            $scope.checkExpandablerow = function (groupKey) {
              
            };
        }
        function load() {
            LoadParentGroupKeys($scope);//return the Parent group keys
            loadMeasures(); // load the measures values - grid headers
            loadMeasuresGroupKeys();
            eliminateGroupKeysNotInParent($scope.parentGroupKeys, $scope.groupKeys);

            

           $scope.selectedGroupKey = $scope.groupKeys[0];
        }
        function retrieveData(groupKey, withSummary) {
            buildFilter($scope);
            buildFilterFromViewScope();

            var query = {
                Filter: filter,
                WithSummary: withSummary,
                GroupKeys: [$scope.selectedGroupKey.value],
                From: $scope.viewScope.fromDate,
                To: $scope.viewScope.toDate,

            };
            return groupKey.gridAPI.retrieveData(query);
        }

        function LoadParentGroupKeys(scope) {
            if (scope == $scope.viewScope) {
                    $scope.parentGroupKeys.push(scope.optionsGroups.selectedvalues);
                return;
            }
            else {
                $scope.parentGroupKeys.push(scope.gridParentScope.optionsGroups.selectedvalues);
                LoadParentGroupKeys(scope.gridParentScope);
            }

        }


        function loadMeasures() {
            for (var prop in CarrierSummaryMeasureEnum) {
                $scope.measures.push(CarrierSummaryMeasureEnum[prop]);
            }
        }

        function loadMeasuresGroupKeys() {
            for (var prop in CarrierSummaryGroupKeysEnum) {
                $scope.groupKeys.push(CarrierSummaryGroupKeysEnum[prop]);
            }
        }

        function eliminateGroupKeysNotInParent(parentGroupKeys, groupKeys) {

            for (var i = 0; i < parentGroupKeys.length; i++) {
                for (var j = 0; j < groupKeys.length; j++)
                    if (parentGroupKeys[i].value == groupKeys[j].value)
                        groupKeys.splice(j, 1);
            }
        }
     
        function buildFilterFromViewScope() {
            if ($scope.viewScope.filter.filter.CustomerIds != null);
            {
                if (filter.CustomerIds == undefined)
                    filter.CustomerIds = [];
                fillArray(filter.CustomerIds, $scope.viewScope.filter.filter.CustomerIds);
            }

            if ($scope.viewScope.filter.filter.SupplierIds != null);
            {
                if (filter.SupplierIds == undefined)
                    filter.SupplierIds = [];
                fillArray(filter.SupplierIds, $scope.viewScope.filter.filter.SupplierIds);
            }

            if ($scope.viewScope.filter.filter.SwitchIds != null);
            {
                if (filter.SwitchIds == undefined)
                    filter.SwitchIds = [];
                fillArray(filter.SwitchIds, $scope.viewScope.filter.filter.SwitchIds);
            }

            if ($scope.viewScope.filter.filter.CodeGroups != null);
            {
                if (filter.CodeGroups == undefined)
                    filter.CodeGroups = [];
                fillArray(filter.CodeGroups, $scope.viewScope.filter.filter.CodeGroups);
            }


        }
        function fillArray(array, data) {
            for (var i = 0; i < data.length; i++) {
                array.push(data[i]);
            }

        }
        function buildFilter(scope) {

            if (scope == $scope.viewScope)
                return;

            if (scope.gridParentScope == $scope.viewScope)
                var parentGroupKeys = scope.gridParentScope.currentSearchCriteria.groupKeys;
            else
                var parentGroupKeys = [scope.gridParentScope.selectedGroupKey];

            for (var i = 0; i < parentGroupKeys.length; i++) {
                var groupKey = parentGroupKeys[i];

                switch (groupKey.value) {
                    case TrafficStatisticGroupKeysEnum.OurZone.value:
                        filter.ZoneIds = [scope.dataItem.GroupKeyValues[i].Id];
                        break;
                    case TrafficStatisticGroupKeysEnum.CustomerId.value:
                        filter.CustomerIds = [scope.dataItem.GroupKeyValues[i].Id];
                        break;
                    case TrafficStatisticGroupKeysEnum.SupplierId.value:
                        filter.SupplierIds = [scope.dataItem.GroupKeyValues[i].Id];
                        break;
                }
            }


            buildFilter(scope.gridParentScope);


        }
    }
    appControllers.controller('CarrierSummaryGridController', CarrierSummaryGridController);

})(appControllers);