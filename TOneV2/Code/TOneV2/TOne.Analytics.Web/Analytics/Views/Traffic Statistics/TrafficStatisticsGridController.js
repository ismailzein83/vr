/// <reference path="ZoneMonitorSettings.html" />
/// <reference path="ZoneMonitor.html" />
TrafficStatisticsGridController.$inject = ['$scope', 'AnalyticsAPIService', 'TrafficStatisticGroupKeysEnum', 'TrafficStatisticsMeasureEnum', 'VRModalService'];
function TrafficStatisticsGridController($scope, AnalyticsAPIService, TrafficStatisticGroupKeysEnum, TrafficStatisticsMeasureEnum, VRModalService) {
    var measures = [];
    var filter = {};
    var notSelectedGroupKeys = [];
    defineScopeObjects();
    defineScopeMethods();
    load();
    
    var selectedGroupKeys = [];
    function defineScopeObjects() {
        $scope.selectedGroupKeys = [];
        $scope.notSelectedGroupKeys = [];
        $scope.measures = measures;
        $scope.currentSearchCriteria = {
            groupKeys: []
        };
        
        $scope.groupKeys = [];
        $scope.menuActions = [{
            name: "CDRs",
            clicked: function (dataItem) {

            var modalSettings = {
                useModalTemplate: true,
                width: "80%",
                maxHeight: "800px"
            };
            var parameters = {
                fromDate: $scope.viewScope.fromDate,
                toDate: $scope.viewScope.toDate,
                customerIds:[],
                zoneIds:[],
                supplierIds:[]
            };
            updateParametersFromGroupKeys(parameters, $scope, dataItem);

            VRModalService.showModal('/Client/Modules/Analytics/Views/CDR/CDRLog.html', parameters, modalSettings);
        }
        }];
    }
    function getNotSelectedGroupKeys(scope) {
        
        if (scope == $scope.viewScope)
            return;
        getSelectedGroupKeys(scope);
        console.log(selectedGroupKeys);
        if (scope.notSelectedGroupKeys.length == $scope.viewScope.groupKeys.length)
            return;
        if ($scope.gridParentScope == scope.viewScope) {
            $scope.notSelectedGroupKeys = scope.groupKeys;
        }
        else
        for (var i = 0; i < scope.selectedGroupKeys.length; i++) {
            if (scope.gridParentScope != $scope.viewScope && scope.gridParentScope.selectedGroupKey.groupKeyEnumValue != scope.selectedGroupKeys[i].groupKeyEnumValue)
                $scope.notSelectedGroupKeys.push(scope.selectedGroupKeys[i]);
            
            //console.log($scope.groupKeys);
           
            
        }
        
        getNotSelectedGroupKeys(scope.gridParentScope);
        console.log($scope.notSelectedGroupKeys);
    }
    function getSelectedGroupKeys(scope) {
       
        if (scope == $scope.viewScope)
            return;
        else {
            $scope.selectedGroupKeys.push(scope.selectedGroupKey);
            getSelectedGroupKeys(scope.gridParentScope);
        }

    }
    function updateParametersFromGroupKeys(parameters, scope, dataItem) {
        var groupKeys = [];
        if (scope == undefined)
            return;
        if (scope == $scope.viewScope)
        {     
             groupKeys = scope.selectedGroupKeys;  
        }
        else {
            groupKeys = [scope.selectedGroupKey];
        }
        
        for (var i = 0; i < groupKeys.length; i++) {
            var groupKey = groupKeys[i];
            switch (groupKey.groupKeyEnumValue) {
                case TrafficStatisticGroupKeysEnum.OurZone.value:
                    parameters.zoneIds.push(dataItem.GroupKeyValues[i].Id);
                                break;
                case TrafficStatisticGroupKeysEnum.CustomerId.value: console.log(dataItem.GroupKeyValues[i].Id);
                                parameters.customerIds.push(dataItem.GroupKeyValues[i].Id);
                                break;
                case TrafficStatisticGroupKeysEnum.SupplierId.value:
                                parameters.supplierIds.push(dataItem.GroupKeyValues[i].Id);
                                break;
            }
        }

        updateParametersFromGroupKeys(parameters, scope.gridParentScope, scope.dataItem);
    }

    
    function defineScopeMethods() {

        $scope.onEntityClicked = function (dataItem) {
            var parentGroupKeys = $scope.viewScope.groupKeys;

            var selectedGroupKeyInParent = $.grep(parentGroupKeys, function (parentGrpKey) {
                return parentGrpKey.groupKeyEnumValue == $scope.selectedGroupKey.groupKeyEnumValue;
            })[0];
            $scope.viewScope.selectEntity(selectedGroupKeyInParent, dataItem.GroupKeyValues[0].Id, dataItem.GroupKeyValues[0].Name)
        };

        $scope.groupKeySelectionChanged = function () {
            if ($scope.selectedGroupKeyIndex != undefined) {
                $scope.selectedGroupKey = $scope.groupKeys[$scope.selectedGroupKeyIndex];
                if (!$scope.selectedGroupKey.isDataLoaded) {
                    
                   
                    getData();
                }
                    
            }
        };
    }

  


    function load() {
       
        loadMeasures();
        loadGroupKeys();
        // $scope.notSelectedGroupKeys = [$scope.selectedGroupKey.groupKeyEnumValue];
       // if ($scope.gridParentScope != $scope.viewScope)
            
        getNotSelectedGroupKeys($scope);
        $scope.selectedGroupKey = $scope.notSelectedGroupKeys[0];
        if ($scope.selectedGroupKey != undefined || $scope.getNotSelectedGroupKeys != undefined)
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

    function getData() {
        if ($scope.notSelectedGroupKeys.length == 0)
            return;
        var withSummary = false;
        var fromRow = 1;
        var toRow = 100;
        buildFilter($scope);
        console.log(filter);
        var getTrafficStatisticSummaryInput = {
            TempTableKey: null,
            Filter: filter,
            WithSummary: withSummary,
            GroupKeys: [$scope.selectedGroupKey.groupKeyEnumValue],
            From: $scope.viewScope.fromDate,
            To: $scope.viewScope.toDate,
            FromRow: fromRow,
            ToRow: toRow,
            OrderBy: 2,
            IsDescending: true
        };
       // console.log($scope.notSelectedGroupKeys);
        var isSucceeded;
        $scope.isGettingData = true;
        $scope.currentSearchCriteria.groupKeys.length = 0;
        angular.forEach($scope.selectedGroupKey, function (group) {
            $scope.currentSearchCriteria.groupKeys.push(group);
        });
        return AnalyticsAPIService.GetTrafficStatisticSummary(getTrafficStatisticSummaryInput).then(function (response) {
            console.log(response);
            angular.forEach(response.Data, function (itm) {
                $scope.selectedGroupKey.data.push(itm);
            });
            $scope.selectedGroupKey.isDataLoaded = true;
        })
            .finally(function () {
                $scope.isGettingData = false;
            });
    }

    function buildFilter(scope) {
       
        if (scope == $scope.viewScope)
            return ;

        if (scope.gridParentScope == $scope.viewScope)
            var parentGroupKeys = scope.gridParentScope.currentSearchCriteria.groupKeys;
        else
        var parentGroupKeys = [scope.gridParentScope.selectedGroupKey];

        for (var i = 0; i < parentGroupKeys.length; i++) {
            var groupKey = parentGroupKeys[i];
            switch(groupKey.groupKeyEnumValue)
            {
                case TrafficStatisticGroupKeysEnum.OurZone.value:
                    filter.ZoneIds = [scope.dataItem.GroupKeyValues[i].Id];
                    break;
                case TrafficStatisticGroupKeysEnum.CustomerId.value: 
                    filter.CustomerIds=[scope.dataItem.GroupKeyValues[i].Id];
                    break;
                case TrafficStatisticGroupKeysEnum.SupplierId.value: 
                    filter.SupplierIds = [scope.dataItem.GroupKeyValues[i].Id];
                    break;
            }
        }
        
        buildFilter(scope.gridParentScope);
       
        
    }


};

appControllers.controller('TrafficStatisticsGridController', TrafficStatisticsGridController);