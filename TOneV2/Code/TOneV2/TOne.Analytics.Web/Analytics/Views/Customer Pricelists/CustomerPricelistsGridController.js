'use strict'
CustomerPricelistsGridController.$inject = ['$scope', 'AnalyticsAPIService', 'TrafficStatisticGroupKeysEnum', 'TrafficMonitorMeasureEnum', 'VRModalService', 'UtilsService', 'LabelColorsEnum', 'AnalyticsService'];
function CustomerPricelistsGridController($scope, AnalyticsAPIService, TrafficStatisticGroupKeysEnum, TrafficMonitorMeasureEnum, VRModalService, UtilsService, LabelColorsEnum, AnalyticsService) {
    var measures = [];
    var filter = {};
    var selectedGroupKeys = [];
    defineScope();
    load();
    function defineScope() {
        $scope.measures = measures;
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
                    customerIds: [],
                    zoneIds: [],
                    supplierIds: [],
                    switchIds: []
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

  
   
    function loadMeasures() {
        for (var prop in TrafficMonitorMeasureEnum) {
            measures.push(TrafficMonitorMeasureEnum[prop]);
        }
    }
   
    function fillArray(array, data) {
        for (var i = 0; i < data.length; i++) {
            array.push(data[i]);
        }

    }
  

};

appControllers.controller('CustomerPricelistsGridController', CustomerPricelistsGridController);