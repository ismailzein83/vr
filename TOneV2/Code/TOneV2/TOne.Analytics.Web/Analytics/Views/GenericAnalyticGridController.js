(function (appControllers) {

    "use strict";

    GenericAnalyticGridController.$inject = ['$scope', 'GenericAnalyticAPIService', 'GenericAnalyticGroupKeysEnum', 'GenericAnalyticMeasureEnum', 'VRModalService', 'UtilsService', 'AnalyticsService'];
    function GenericAnalyticGridController($scope, GenericAnalyticAPIService, GenericAnalyticGroupKeysEnum, GenericAnalyticMeasureEnum, VRModalService, UtilsService, AnalyticsService) {
        var filter = {};
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
              
            };
        }

        function retrieveData(groupKey, withSummary) {
            buildFilter($scope);
            //buildFilterFromViewScope();


            var query = {
                GroupFields: [$scope.selectedGroupKey.value],
                MeasureFields: measures,
                FromTime: $scope.viewScope.fromDate,
                ToTime: $scope.viewScope.toDate,
            };

            return groupKey.gridAPI.retrieveData(query);
        }


        function loadQueryMeasures() {
            if (measureFields.length == 0)
                for (var prop in GenericAnalyticMeasureEnum) {
                    measureFields.push(GenericAnalyticMeasureEnum[prop]);
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
                    case GenericAnalyticGroupKeysEnum.ZoneId.value:
                        filter.ZoneIds = [scope.dataItem.DimensionValues[i].Id];
                        break;
                    case GenericAnalyticGroupKeysEnum.CustomerId.value:
                        filter.CustomerIds = [scope.dataItem.DimensionValues[i].Id];
                        break;
                    case GenericAnalyticGroupKeysEnum.SupplierId.value:
                        filter.SupplierIds = [scope.dataItem.DimensionValues[i].Id];
                        break;
                }
            }


            buildFilter(scope.gridParentScope);


        }

        function load() {
            loadGroupKeys();// Load all group keys
            loadMeasures();
            loadQueryMeasures();
            //LoadParentGroupKeys($scope.gridParentScope);//return the Parent group keys        

           //Eliminate Parent group keys from group keys
            // addFunctionsGroupKey();

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
           
            if ($scope.groupKeys.length > 0)
                $scope.selectedGroupKey = $scope.groupKeys[0];
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
  
            //for (var i = 0; i < $scope.groupKeys.length; i++) {
            //    $scope.groupKeys[i].onGridReady = function (api) {
            //        $scope.groupKeys[i].gridAPI = api;
            //        if ($scope.selectedGroupKey == $scope.groupKeys[i])
            //            retrieveData($scope.groupKeys[i], false);
            //    };
            //    $scope.groupKeys[i].dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
            //        return GenericAnalyticAPIService.GetFiltered(dataRetrievalInput).then(function (response) {

            //            $scope.selectedGroupKey.isDataLoaded = true;
            //            console.log("TTEEESST");
            //            onResponseReady(response);
            //        })
            //    };
            //}
           
        }

        //function loadGroupKeys() {
        //    for (var prop in GenericAnalyticGroupKeysEnum) {
        //        $scope.groupKeys.push(GenericAnalyticGroupKeysEnum[prop]);
        //    }
        //}

        function LoadParentGroupKeys(scope) {
            if (scope == $scope.viewScope) {
                $scope.parentGroupKeys = scope.selectedGroupKeys;
                return;
            }
            else {
                $scope.parentGroupKeys.push(scope.gridParentScope.selectedGroupKeys);
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