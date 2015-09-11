(function (appControllers) {

    "use strict";

    GenericAnalyticSubGridController.$inject = ['$scope', 'GenericAnalyticAPIService', 'GenericAnalyticDimensionEnum', 'AnalyticsService'];
    function GenericAnalyticSubGridController($scope, GenericAnalyticAPIService, GenericAnalyticDimensionEnum, analyticsService) {
        var filter = {};
        var measureFields = [];
        var selectedGroupKeys = [] , parentGroupKeys = [];
        
        load();

        function defineScope() {

            measureFields = analyticsService.getGenericAnalyticMeasureValues();
            $scope.measures = $scope.viewScope.measures;

            $scope.selectedGroupKey;
            $scope.dimensions = [];

            $scope.groupKeySelectionChanged = function () {

                if ($scope.selectedGroupKeyIndex != undefined) {
                    $scope.selectedGroupKey = $scope.dimensions[$scope.selectedGroupKeyIndex];

                    
                    if (!$scope.selectedGroupKey.isDataLoaded && $scope.selectedGroupKey.gridAPI != undefined) {
                        retrieveData($scope.selectedGroupKey, false);
                    }

                }
            };


            $scope.checkExpandablerow = function () {
             if ($scope.dimensions.length > 1)
                    return true;
                else 
                    return false;
            };
        }

        function retrieveData(groupKey, withSummary) {
            filter = {};
            buildFilter($scope);

            var filterResult = [];

            for (var prop in filter)
                filterResult.push({ Dimension: parseInt(prop), FilterValues: filter[prop] });
            var query = {
                Filters: filterResult,
                DimensionFields: [$scope.selectedGroupKey.value],
                MeasureFields: measureFields,
                FromTime: $scope.viewScope.fromDate,
                ToTime: $scope.viewScope.toDate
            };
            return groupKey.gridAPI.retrieveData(query);
        }

        function buildFilter(scope) {

            if (scope == $scope.viewScope)
                return;
            var parentGroupKeys;
            if (scope.gridParentScope == $scope.viewScope)
                parentGroupKeys = scope.gridParentScope.selectedGroupKeys;
            else
                parentGroupKeys = [scope.gridParentScope.selectedGroupKey];



            for (var i = 0; i < parentGroupKeys.length; i++) {
                var groupKey = parentGroupKeys[i];

                for (var item in GenericAnalyticDimensionEnum) {
                    if (GenericAnalyticDimensionEnum.hasOwnProperty(item)) {
                        if (groupKey.value == GenericAnalyticDimensionEnum[item].value) {
                            if (filter[groupKey.value] === undefined) filter[groupKey.value] = [];
                                filter[groupKey.value].push(scope.dataItem.DimensionValues[i].Id);
                        }
                    }
                }
            }

            buildFilter(scope.gridParentScope);
        }

        function load() {
            defineScope();
            loadGroupKeys();
            $scope.selectedGroupKey = $scope.dimensions[0];
        }

        function loadGroupKeys() {
            for (var prop in GenericAnalyticDimensionEnum) {
                var groupKey = {
                    name: GenericAnalyticDimensionEnum[prop].name,
                    value: GenericAnalyticDimensionEnum[prop].value,
                    data: [],
                    isDataLoaded: false
                };
               
                addGroupKeyIfNotExistsInParent(groupKey);
            }

            LoadParentGroupKeys($scope.gridParentScope);
            eliminateGroupKeysNotInParent(parentGroupKeys, $scope.dimensions);
        }

        function addGroupKeyIfNotExistsInParent(dimension) {
            var parentGroupKeys = $scope.viewScope.selectedGroupKeys;
            for (var i = 0; i < parentGroupKeys.length; i++) {
                if (parentGroupKeys[i].value == dimension.value)
                    return;
            }
            dimension.onGridReady = function (api) {
                dimension.gridAPI = api;
                if ($scope.selectedGroupKey == dimension)
                        retrieveData(dimension, false);
                };
                dimension.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return GenericAnalyticAPIService.GetFiltered(dataRetrievalInput).then(function (response) {
                        $scope.selectedGroupKey.isDataLoaded = true;
                        onResponseReady(response);
                    })
                };
                $scope.dimensions.push(dimension);         
        }

        function LoadParentGroupKeys(scope) {
            if (scope == $scope.viewScope) 
                {
                    for (var i = 0; i < scope.selectedGroupKeys.length; i++) {
                        parentGroupKeys.push(scope.selectedGroupKeys[i]);
                    }
                    return;
                }
            else {
                parentGroupKeys.push(scope.selectedGroupKey);
                LoadParentGroupKeys(scope.gridParentScope);
            }
        }

        function eliminateGroupKeysNotInParent(parentGroupKeys, groupKeys) {
            for (var i = 0; i < parentGroupKeys.length; i++) {
                for (var j = 0; j < groupKeys.length; j++)
                    if (parentGroupKeys[i].value == groupKeys[j].value)
                        groupKeys.splice(j, 1);
            }
        }

    }
    appControllers.controller('GenericAnalyticSubGridController', GenericAnalyticSubGridController);

})(appControllers);