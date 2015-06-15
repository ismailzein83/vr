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
        
        console.log("hjds"+$scope.viewScope.value);
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

    function getNotSelectedGroupKeys(groupKeys, length) {
        if (length <= 0)
            return; 
        if (checkSelectedGroupKeys($scope.selectedGroupKeys, groupKeys, length, $scope.selectedGroupKeys.length - 1) == 0 && !contains(groupKeys[length - 1], $scope.notSelectedGroupKeys))
                $scope.notSelectedGroupKeys.push($scope.groupKeys[length-1]);
        getNotSelectedGroupKeys(groupKeys, length - 1);
    }
    function checkSelectedGroupKeys(selectedGroupKeys, groupKeys,lengthGroupKey, length) {
        if (length <= 0)
            return 0;
        if (groupKeys[lengthGroupKey-1].groupKeyEnumValue == selectedGroupKeys[length-1].groupKeyEnumValue) {
            return 1 + checkSelectedGroupKeys(selectedGroupKeys, groupKeys, lengthGroupKey, length - 1);
        }
        else
            return checkSelectedGroupKeys(selectedGroupKeys, groupKeys, lengthGroupKey, length - 1);
 
    }


    function getSelectedGroupKeys(scope) { 
        if (scope == $scope.viewScope){
            for (var i = 0; i < scope.selectedGroupKeys.length; i++)
            {
                $scope.selectedGroupKeys.push(scope.selectedGroupKeys[i]);
            }
            return;
        }
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
                $scope.selectedGroupKey = $scope.notSelectedGroupKeys[$scope.selectedGroupKeyIndex];
                if (!$scope.selectedGroupKey.isDataLoaded) {
                    $scope.selectedGroupKeys = [];
                    getSelectedGroupKeys($scope.gridParentScope);
                    getNotSelectedGroupKeys($scope.groupKeys, $scope.groupKeys.length);
                    getData();
                }
                    
            }
        };
    }

  


    function load() {
        loadMeasures();
        loadGroupKeys();
        $scope.selectedGroupKeys = [];
        getSelectedGroupKeys($scope.gridParentScope);
        getNotSelectedGroupKeys($scope.groupKeys, $scope.groupKeys.length);

       // getNotSelectedGroupKeys();
       $scope.selectedGroupKey = $scope.notSelectedGroupKeys[0];
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
            title: "Customer",
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
        addGroupKeyIfNotExistsInParent({
            title: "Switch",
            groupKeyEnumValue: TrafficStatisticGroupKeysEnum.Switch.value,
            data: [],
            isDataLoaded: false,
            gridHeader: "Switch"
        });
        addGroupKeyIfNotExistsInParent({
            title: "PortIn",
            groupKeyEnumValue: TrafficStatisticGroupKeysEnum.PortIn.value,
            data: [],
            isDataLoaded: false,
            gridHeader: "PortIn"
        });
        addGroupKeyIfNotExistsInParent({
            title: "PortOut",
            groupKeyEnumValue: TrafficStatisticGroupKeysEnum.PortOut.value,
            data: [],
            isDataLoaded: false,
            gridHeader: "PortOut"
        });
        if ($scope.groupKeys.length > 0)
            $scope.selectedGroupKey = $scope.groupKeys[0];

    }
    function contains(obj, list) {
        var i;
        for (i = 0; i < list.length; i++) {
            if (list[i] === obj) {
                return true;
            }
        }

        return false;
    }
    function addGroupKeyIfNotExistsInParent(groupKey) {
        var parentGroupKeys = $scope.viewScope.currentSearchCriteria.groupKeys;
        if ($.grep(parentGroupKeys, function (parentGrpKey) {
            return parentGrpKey.groupKeyEnumValue == groupKey.groupKeyEnumValue;
        }).length == 0)
            $scope.groupKeys.push(groupKey);
    }

    function getData() {
        //if ($scope.notSelectedGroupKeys.length == 0)
        //    return;
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
        console.log($scope.selectedGroupKey.groupKeyEnumValue);
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
                case TrafficStatisticGroupKeysEnum.Switch.value:
                    filter.Switch = [scope.dataItem.GroupKeyValues[i].Id];
                    break;
            }
        }
        
        buildFilter(scope.gridParentScope);
       
        
    }


};

appControllers.controller('TrafficStatisticsGridController', TrafficStatisticsGridController);