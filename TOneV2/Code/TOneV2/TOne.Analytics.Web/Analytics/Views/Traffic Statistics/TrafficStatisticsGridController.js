'use strict'
/// <reference path="ZoneMonitorSettings.html" />
/// <reference path="ZoneMonitor.html" />
TrafficStatisticsGridController.$inject = ['$scope', 'AnalyticsAPIService', 'TrafficStatisticGroupKeysEnum', 'TrafficStatisticsMeasureEnum', 'VRModalService','UtilsService','LabelColorsEnum'];
function TrafficStatisticsGridController($scope, AnalyticsAPIService, TrafficStatisticGroupKeysEnum, TrafficStatisticsMeasureEnum, VRModalService, UtilsService, LabelColorsEnum) {
    var measures = [];
    var filter = {};
    var selectedGroupKeys = [];
    defineScope();
    load();
    function defineScope() {
        
        $scope.parentGroupKeys = [];
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
                supplierIds: [],
                switchIds:[]
            };
            updateParametersFromGroupKeys(parameters, $scope, dataItem);
            VRModalService.showModal('/Client/Modules/Analytics/Views/CDR/CDRLog.html', parameters, modalSettings);
        }
        }];
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
            if ($scope.groupKeys.length == 2 && groupKey.value == TrafficStatisticGroupKeysEnum.OurZone.value && ($scope.groupKeys[0].value == TrafficStatisticGroupKeysEnum.CodeGroup.value || $scope.groupKeys[1].value == TrafficStatisticGroupKeysEnum.CodeGroup.value))//only if zone and codegroup remains in groupkeys
                return false;
            else if ($scope.groupKeys.length > 1)
                return true;
            else if ($scope.selectedGroupKey.value == TrafficStatisticGroupKeysEnum.SupplierId.value)
                return true;
            else
                return false;
        };
        $scope.getColor = function (dataItem, coldef) {
            if (coldef.tag.value == TrafficStatisticsMeasureEnum.ACD.value)
                return getACDColor(dataItem.ACD, dataItem.Attempts);
        }

    }
    function load() {
        loadMeasures();
        loadGroupKeys();
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
    function applySupplierZoneIdRule() {
        for (var i = 0; i < $scope.parentGroupKeys.length; i++) {
            if ($scope.parentGroupKeys[i].value == TrafficStatisticGroupKeysEnum.SupplierId.value)
                return;
        }
        removeSupplierIdFromGroupKeys();

    }
    function removeSupplierIdFromGroupKeys() {
        for (var i = 0; i < $scope.groupKeys.length; i++) {
            if ($scope.groupKeys[i].value == TrafficStatisticGroupKeysEnum.SupplierZoneId.value) {
                $scope.groupKeys.splice(i, 1);
            }
        }
    }
    function applyCodeGroupRule() {
        for (var i = 0; i < $scope.parentGroupKeys.length; i++) {
            if ($scope.parentGroupKeys[i].value == TrafficStatisticGroupKeysEnum.OurZone.value)
                removeCodeGroupFromGroupKeys();
        }
  
    }
    function getACDColor(acdValue, attemptsValue) {
        if (attemptsValue > $scope.viewScope.attampts && acdValue < $scope.viewScope.acd)
            return LabelColorsEnum.Warning.Color;
        //if (status === BPInstanceStatusEnum.Running.value) return LabelColorsEnum.Info.Color;
        //if (status === BPInstanceStatusEnum.ProcessFailed.value) return LabelColorsEnum.Error.Color;
        //if (status === BPInstanceStatusEnum.Completed.value) return LabelColorsEnum.Success.Color;
        //if (status === BPInstanceStatusEnum.Aborted.value) return LabelColorsEnum.Warning.Color;
        //if (status === BPInstanceStatusEnum.Suspended.value) return LabelColorsEnum.Warning.Color;
        //if (status === BPInstanceStatusEnum.Terminated.value) return LabelColorsEnum.Error.Color;

        // return LabelColorsEnum.Info.Color;
    };
    function removeCodeGroupFromGroupKeys() {
        for (var i = 0; i < $scope.groupKeys.length; i++) {
            if ($scope.groupKeys[i].value == TrafficStatisticGroupKeysEnum.CodeGroup.value) {
                $scope.groupKeys.splice(i, 1);
            }
        }
    }
    function eliminateGroupKeysNotInParent() {
        
        for (var i = 0; i < $scope.parentGroupKeys.length; i++) {
            for(var j=0;j< $scope.groupKeys.length;j++)
                if($scope.parentGroupKeys[i].value==$scope.groupKeys[j].value)
                    $scope.groupKeys.splice(j, 1);
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
            switch (groupKey.value) {
                case TrafficStatisticGroupKeysEnum.OurZone.value:
                    parameters.zoneIds.push(dataItem.GroupKeyValues[i].Id);
                                break;
                case TrafficStatisticGroupKeysEnum.CustomerId.value:
                                parameters.customerIds.push(dataItem.GroupKeyValues[i].Id);
                                break;
                case TrafficStatisticGroupKeysEnum.SupplierId.value:
                                parameters.supplierIds.push(dataItem.GroupKeyValues[i].Id);
                                break;
                case TrafficStatisticGroupKeysEnum.Switch.value:
                    parameters.switchIds.push(dataItem.GroupKeyValues[i].Id);
                    break;
            }
        }

        updateParametersFromGroupKeys(parameters, scope.gridParentScope, scope.dataItem);
    }   
    function loadMeasures() {
        for (var prop in TrafficStatisticsMeasureEnum) {
            measures.push(TrafficStatisticsMeasureEnum[prop]);
        }
    }
    function loadGroupKeys() {
        for (var prop in TrafficStatisticGroupKeysEnum) {
            var groupKey = {
                title: TrafficStatisticGroupKeysEnum[prop].title,
                value: TrafficStatisticGroupKeysEnum[prop].value,
                data: [],
                isDataLoaded: false,
                gridHeader: TrafficStatisticGroupKeysEnum[prop].gridHeader
            };
            
            addGroupKeyIfNotExistsInParent(groupKey); 
        }
        if ($scope.groupKeys.length > 0)
            $scope.selectedGroupKey = $scope.groupKeys[0];

        LoadParentGroupKeys($scope.gridParentScope);
        applySupplierZoneIdRule();
        applyCodeGroupRule();
        eliminateGroupKeysNotInParent();
     
    }
    function addGroupKeyIfNotExistsInParent(groupKey) {
        var parentGroupKeys = $scope.viewScope.currentSearchCriteria.groupKeys;
        if ($.grep(parentGroupKeys, function (parentGrpKey) {
            return parentGrpKey.value == groupKey.value;
        }).length == 0) {
            groupKey.onGridReady = function (api) {
                groupKey.gridAPI = api;
                if ($scope.selectedGroupKey == groupKey)
                    retrieveData(groupKey, false);
            };
            groupKey.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return AnalyticsAPIService.GetTrafficStatisticSummary(dataRetrievalInput).then(function (response) {
                    $scope.selectedGroupKey.isDataLoaded = true;
                    onResponseReady(response);
                })
            };
            $scope.groupKeys.push(groupKey);
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
            return ;

        if (scope.gridParentScope == $scope.viewScope)
            var parentGroupKeys = scope.gridParentScope.currentSearchCriteria.groupKeys;
        else
        var parentGroupKeys = [scope.gridParentScope.selectedGroupKey];

        for (var i = 0; i < parentGroupKeys.length; i++) {
            var groupKey = parentGroupKeys[i];
           
            switch (groupKey.value)
            {
                case TrafficStatisticGroupKeysEnum.OurZone.value:
                    filter.ZoneIds=[scope.dataItem.GroupKeyValues[i].Id];
                    break;
                case TrafficStatisticGroupKeysEnum.CustomerId.value: 
                    filter.CustomerIds=[scope.dataItem.GroupKeyValues[i].Id];
                    break;
                case TrafficStatisticGroupKeysEnum.SupplierId.value: 
                    filter.SupplierIds=[scope.dataItem.GroupKeyValues[i].Id];
                    break;
                case TrafficStatisticGroupKeysEnum.Switch.value:
                    filter.SwitchIds=[scope.dataItem.GroupKeyValues[i].Id];
                    break;
                case TrafficStatisticGroupKeysEnum.CodeGroup.value:
                    filter.CodeGroups=[scope.dataItem.GroupKeyValues[i].Id];
                    break;
                case TrafficStatisticGroupKeysEnum.PortIn.value:
                    filter.PortIn = [scope.dataItem.GroupKeyValues[i].Id];
                    break;
                case TrafficStatisticGroupKeysEnum.PortOut.value:
                    filter.PortOut = [scope.dataItem.GroupKeyValues[i].Id];
                    break;
                case TrafficStatisticGroupKeysEnum.SupplierZoneId.value:
                    filter.SupplierZoneId = [scope.dataItem.GroupKeyValues[i].Id];
                    break;
                case TrafficStatisticGroupKeysEnum.GateWayIn.value: console.log(scope.dataItem);
                    filter.GateWayIn = [scope.dataItem.GroupKeyValues[i].Id];
                    break;
                case TrafficStatisticGroupKeysEnum.GateWayOut.value: 
                    filter.GateWayOut = [scope.dataItem.GroupKeyValues[i].Id];
                    break;
                case TrafficStatisticGroupKeysEnum.CodeBuy.value:
                    filter.CodeBuy = [scope.dataItem.GroupKeyValues[i].Id];
                    break;
                case TrafficStatisticGroupKeysEnum.CodeSales.value:
                    filter.CodeSales = [scope.dataItem.GroupKeyValues[i].Id];
                    break;
            }
        }
       
        
        buildFilter(scope.gridParentScope);
       
        
    }
};

appControllers.controller('TrafficStatisticsGridController', TrafficStatisticsGridController);