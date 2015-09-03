(function (appControllers) {

    "use strict";

    function filterFactoryObj(trafficStatisticGroupKeysEnum) {

        function validateParam(arr) {
            if (arr === undefined)
                return [];
            if (Object.prototype.toString.call(arr) === "[object Array]") {
                return arr;
            }
            return [arr];
        }

        var filter = function (customerIds, supplierIds, switchIds, codeGroups, zoneIds) {
            this.customerIds = validateParam(customerIds);
            this.supplierIds = validateParam(supplierIds);
            this.switchIds = validateParam(switchIds);
            this.codeGroups = validateParam(codeGroups);
            this.zoneIds = validateParam(zoneIds);
            this.portIn = [];
            this.portOut = [];
            this.supplierZoneId = [];
            this.gateWayIn = [];
            this.gateWayOut = [];
            this.codeBuy = [];
            this.codeSales = [];
        };

        function buildFilter(scope, viewScope, filterObj) {

            if (scope === viewScope)
                return;

            var parentGroupKeys;

            if (scope.gridParentScope === viewScope)
                parentGroupKeys = scope.gridParentScope.currentSearchCriteria.groupKeys;
            else
                parentGroupKeys = [scope.gridParentScope.selectedGroupKey];

            for (var i = 0; i < parentGroupKeys.length; i++) {
                var groupKey = parentGroupKeys[i];

                for (var item in trafficStatisticGroupKeysEnum) {
                    if (trafficStatisticGroupKeysEnum.hasOwnProperty(item)) {
                        if (groupKey.value === trafficStatisticGroupKeysEnum[item].value) {
                            if (filterObj.hasOwnProperty(trafficStatisticGroupKeysEnum[item].filterProp))
                                filterObj[trafficStatisticGroupKeysEnum[item].filterProp].push(scope.dataItem.GroupKeyValues[i].Id);
                        }
                    }
                }
            }

            buildFilter(scope.gridParentScope, viewScope, filterObj);
        }

        filter.prototype.build = function (scope) {
            buildFilter(scope, scope.viewScope, this);
        };

        return filter;
    }

    filterFactoryObj.$inject = ['TrafficStatisticGroupKeysEnum'];
    appControllers.factory('Analytics_FilterFactory', filterFactoryObj);

})(appControllers);