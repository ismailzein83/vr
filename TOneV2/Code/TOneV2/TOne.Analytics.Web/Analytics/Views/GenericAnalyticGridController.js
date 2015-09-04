(function (appControllers) {

    "use strict";

    GenericAnalyticGridController.$inject = ['$scope', 'GenericAnalyticAPIService', 'GenericAnalyticDimensionEnum'];
    function GenericAnalyticGridController($scope, GenericAnalyticAPIService, GenericAnalyticDimensionEnum) {
        var filter = [];
        var measureFields = [];
        var selectedGroupKeys = [] , parentGroupKeys = [];
        
        load();

        function defineScope() {

            measureFields = $scope.viewScope.measureFields;
            $scope.measures = $scope.viewScope.measures;

            $scope.selectedGroupKey;
            $scope.groupKeys = [];

            $scope.groupKeySelectionChanged = function () {

                if ($scope.selectedGroupKeyIndex != undefined) {
                    $scope.selectedGroupKey = $scope.groupKeys[$scope.selectedGroupKeyIndex];

                    
                    if (!$scope.selectedGroupKey.isDataLoaded && $scope.selectedGroupKey.gridAPI != undefined) {
                        retrieveData($scope.selectedGroupKey, false);
                    }

                }
            };


            $scope.checkExpandablerow = function (groupKey) {
             if ($scope.groupKeys.length > 1)
                    return true;
                else 
                    return false;
            };
        }

        function retrieveData(groupKey, withSummary) {
            buildFilter($scope);

            var query = {
                Filters: filter,
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
                            var obj = { Dimension: groupKey.value };
                            obj.FilterValues = [scope.dataItem.DimensionValues[i].Id];
                            filter.push(obj);
                        }
                    }
                }
            }

            buildFilter(scope.gridParentScope);

        }

        function load() {
            defineScope();
            loadGroupKeys();
            $scope.selectedGroupKey = $scope.groupKeys[0];
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
            eliminateGroupKeysNotInParent(parentGroupKeys, $scope.groupKeys);
        }

        function addGroupKeyIfNotExistsInParent(groupKey) {
            var parentGroupKeys = $scope.viewScope.selectedGroupKeys;
            for (var i = 0; i < parentGroupKeys.length; i++) {
                if (parentGroupKeys[i].value == groupKey.value)
                    return;
            }
                groupKey.onGridReady = function (api) {
                    groupKey.gridAPI = api;
                    if ($scope.selectedGroupKey == groupKey)
                        retrieveData(groupKey, false);
                };
                groupKey.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return GenericAnalyticAPIService.GetFiltered(dataRetrievalInput).then(function (response) {
                        $scope.selectedGroupKey.isDataLoaded = true;
                        onResponseReady(response);
                    })
                };
                $scope.groupKeys.push(groupKey);         
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
    appControllers.controller('GenericAnalyticGridController', GenericAnalyticGridController);

})(appControllers);