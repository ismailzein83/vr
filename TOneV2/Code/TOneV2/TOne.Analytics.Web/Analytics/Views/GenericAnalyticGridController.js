(function (appControllers) {

    "use strict";

    GenericAnalyticGridController.$inject = ['$scope', 'GenericAnalyticAPIService', 'GenericAnalyticGroupKeysEnum', 'GenericAnalyticMeasureEnum', 'VRModalService', 'UtilsService', 'AnalyticsService'];
    function GenericAnalyticGridController($scope, GenericAnalyticAPIService, GenericAnalyticGroupKeysEnum, GenericAnalyticMeasureEnum, VRModalService, UtilsService, AnalyticsService) {
        var filter = [];
        var measures = [];
        var measureFields = [];
        var selectedGroupKeys = [];
        defineScope();
        load();

        function defineScope() {

            $scope.parentGroupKeys = [];
            $scope.measures = measureFields;
            $scope.selectedGroupKey;
            $scope.measuresGroupKeys = [];
            $scope.currentSearchCriteria = {
                groupKeys: []
            };
            $scope.groupKeys = [];

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
                MeasureFields: measures,
                FromTime: $scope.viewScope.fromDate,
                ToTime: $scope.viewScope.toDate
            };
            console.log(filter);
            return groupKey.gridAPI.retrieveData(query);
        }

        //scope.dataItem.GroupKeyValues
        function loadQueryMeasures() {
            if (measureFields.length == 0)
                for (var prop in GenericAnalyticMeasureEnum) {
                    measureFields.push(GenericAnalyticMeasureEnum[prop]);
                }
        }

        function buildFilter(scope) {

            if (scope == $scope.viewScope)
                return;

            if (scope.gridParentScope == $scope.viewScope)
                var parentGroupKeys = scope.gridParentScope.selectedGroupKeys;
            else
                var parentGroupKeys = [scope.gridParentScope.selectedGroupKey];



            for (var i = 0; i < parentGroupKeys.length; i++) {
                var groupKey = parentGroupKeys[i];

                for (var item in GenericAnalyticGroupKeysEnum) {
                    if (GenericAnalyticGroupKeysEnum.hasOwnProperty(item)) {
                        if (groupKey.value == GenericAnalyticGroupKeysEnum[item].value) {
                            var obj = { Dimension: groupKey.value };
                            obj.FilterValues = [scope.dataItem.DimensionValues[i].Id];
                            filter.push(obj);
                        }
                    }
                }

                //switch (groupKey.value) {
                //    case GenericAnalyticGroupKeysEnum.ZoneId.value:
                //        filter.ZoneIds = [scope.dataItem.DimensionValues[i].Id];
                //        break;
                //    case GenericAnalyticGroupKeysEnum.CustomerId.value:
                //        filter.CustomerIds = [scope.dataItem.DimensionValues[i].Id];
                //        break;
                //    case GenericAnalyticGroupKeysEnum.SupplierId.value:
                //        filter.SupplierIds = [scope.dataItem.DimensionValues[i].Id];
                //        break;
                //}
            }

            buildFilter(scope.gridParentScope);

        }

        function load() {
            loadGroupKeys();// Load all group keys
            loadMeasures();
            loadQueryMeasures();

           $scope.selectedGroupKey = $scope.groupKeys[0];
        }

        function loadMeasures() {
            for (var prop in GenericAnalyticMeasureEnum) {
                measures.push(GenericAnalyticMeasureEnum[prop].value);
            }
        }

        function loadGroupKeys() {
            for (var prop in GenericAnalyticGroupKeysEnum) {
                var groupKey = {
                    name: GenericAnalyticGroupKeysEnum[prop].name,
                    value: GenericAnalyticGroupKeysEnum[prop].value,
                    data: [],
                    isDataLoaded: false,
                    propertyName: GenericAnalyticGroupKeysEnum[prop].propertyName
                };
               
                addGroupKeyIfNotExistsInParent(groupKey);
            }
           
            //if ($scope.groupKeys.length > 0)
            //    $scope.selectedGroupKey = $scope.groupKeys[0];
            LoadParentGroupKeys($scope.gridParentScope);
            eliminateGroupKeysNotInParent($scope.parentGroupKeys, $scope.groupKeys);
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
                        $scope.parentGroupKeys.push(scope.selectedGroupKeys[i]);
                    }
                    return;
                }
            else {
                $scope.parentGroupKeys.push(scope.selectedGroupKey);
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