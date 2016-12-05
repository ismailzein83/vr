(function (appControllers) {

    "use strict";

    GenericAnalyticSubGridController.$inject = ['$scope', 'GenericAnalyticAPIService', 'GenericAnalyticDimensionEnum','GenericAnalyticMeasureEnum', 'GenericAnalyticService'];
    function GenericAnalyticSubGridController($scope, GenericAnalyticAPIService, GenericAnalyticDimensionEnum, GenericAnalyticMeasureEnum, GenericAnalyticService) {
        var filter = {};
        var measureFields = [];
        var parentGroupKeys = [];
        
        load();

        function defineScope() {
            Object.getOwnPropertyNames($scope.dataItem.MeasureValues).forEach(function (item) {
                measureFields.push(GenericAnalyticMeasureEnum[item].value);
            });
            $scope.measures = Object.getOwnPropertyNames($scope.dataItem.MeasureValues);
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

            $scope.getColor = function (dataItem, coldef) {
                if ($scope.gridParentScope.params != undefined)
                    return GenericAnalyticService.getMeasureColor(dataItem, coldef, $scope.gridParentScope.params);
                else
                    return GenericAnalyticService.getMeasureColor(dataItem, coldef, $scope.viewScope.params);
            };
        }

        function retrieveData(groupKey, withSummary) {
            filter = {};

            buildFilter($scope);
            
            $scope.selectedfilters = $scope.gridParentScope.selectedfilters;
            var filterResult = [];

            for (var prop in  $scope.gridParentScope.selectedfilters) {
                filterResult.push($scope.gridParentScope.selectedfilters[prop]);
            }

            for (var prop in filter) {

                var x = 0;

                for (var i = 0, len = filterResult.length; i < len; i++) {
                    if (parseInt(prop) == filterResult[i].Dimension) {
                        var y = 0;
                        for (var j = 0, len2 = filterResult[i].FilterValues.length; j < len2; j++) {

                            if (filterResult[i].FilterValues[j] == filter[prop][0]) {
                                y = 1;
                                break;
                            }
                        }
                        if (y == 0)
                            filterResult[i].FilterValues.push(filter[prop][0]);
                        x = 1;
                    }
                }

                if(x == 0)
                    filterResult.push({ Dimension: parseInt(prop), FilterValues: filter[prop] });
            }

            var query = {
                Filters: filterResult,
                DimensionFields: [$scope.selectedGroupKey.value],
                MeasureFields: measureFields,
                FromTime: $scope.viewScope.fromDate,
                ToTime: $scope.viewScope.toDate,
                Currency: $scope.viewScope.Currency
            };
            return groupKey.gridAPI.retrieveData(query);
        }

        function buildFilter(scope) {

            if (scope == $scope.viewScope)
                return;

            var parentGroupKeys;
            if (scope.gridParentScope == $scope.viewScope) 
                parentGroupKeys = scope.gridParentScope.selectedobject.selecteddimensions;
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
            for (var prop in $scope.viewScope.groupKeys) {
                var groupKey = {
                    name: $scope.viewScope.groupKeys[prop].name,
                    value: $scope.viewScope.groupKeys[prop].value,
                    data: [],
                    isDataLoaded: false
                };
               
                addGroupKeyIfNotExistsInParent(groupKey);
            }

            LoadParentGroupKeys($scope.gridParentScope);
            eliminateGroupKeysNotInParent(parentGroupKeys, $scope.dimensions);
        }

        function addGroupKeyIfNotExistsInParent(dimension) {
            var parentGroupKeys = $scope.viewScope.selectedobject.selecteddimensions;
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
                for (var i = 0; i < scope.selectedobject.selecteddimensions.length; i++) {
                    parentGroupKeys.push(scope.selectedobject.selecteddimensions[i]);
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