(function(appControllers) {

    "use strict";

    analyticsServiceObj.$inject = ['TrafficStatisticGroupKeysEnum', 'GenericAnalyticGroupKeysEnum', 'PeriodEnum', 'UtilsService', 'VRModalService', 'ReleaseCodeMeasureEnum'];

    function analyticsServiceObj(trafficStatisticGroupKeysEnum, GenericAnalyticGroupKeysEnum, periodEnum, utilsService, vrModalService, releaseCodeMeasureEnum) {
        
        function getTrafficStatisticGroupKeys() {
            var groupKeys = [];
            for (var prop in trafficStatisticGroupKeysEnum) {
                if (trafficStatisticGroupKeysEnum[prop].isShownInGroupKey)
                    groupKeys.push(trafficStatisticGroupKeysEnum[prop]);
            }
            return groupKeys;
        }

        
        function getGenericAnalyticGroupKeys() {
            var groupKeys = [];
            for (var prop in GenericAnalyticGroupKeysEnum) {
                    groupKeys.push(GenericAnalyticGroupKeysEnum[prop]);
            }
            return groupKeys;
        }

        function getDefaultTrafficStatisticGroupKeys() {
            var groupKeys = [];
            groupKeys.push(trafficStatisticGroupKeysEnum.OurZone);
            return groupKeys;
        }

        function getPeriods() {
            return utilsService.getArrayEnum(periodEnum);
        }

        function loadCdrParameters(parameters, dataItemGroupKeyValues, filterGroupKeys) {
            for (var i = 0; i < filterGroupKeys.length; i++) {
                var groupKey = filterGroupKeys[i];
                switch (groupKey.value) {
                    case trafficStatisticGroupKeysEnum.OurZone.value:
                        parameters.zoneIds = [dataItemGroupKeyValues[i].Id];
                        break;
                    case trafficStatisticGroupKeysEnum.CustomerId.value:
                        parameters.customerIds = [dataItemGroupKeyValues[i].Id];
                        break;
                    case trafficStatisticGroupKeysEnum.SupplierId.value:
                        parameters.supplierIds = [dataItemGroupKeyValues[i].Id];
                        break;
                    case trafficStatisticGroupKeysEnum.Switch.value:
                        parameters.switchIds = [dataItemGroupKeyValues[i].Id];
                        break;
                    case trafficStatisticGroupKeysEnum.PortIn.value:
                        parameters.PortIn = [dataItemGroupKeyValues[i].Id];
                        break;
                    case trafficStatisticGroupKeysEnum.PortOut.value:
                        parameters.PortOut = [dataItemGroupKeyValues[i].Id];
                        break;
                }
            }

        }

        function showCdrLogModal(parameters, dataItemGroupKeyValues, filterGroupKeys) {
            
            loadCdrParameters(parameters, dataItemGroupKeyValues, filterGroupKeys);

            vrModalService.showModal('/Client/Modules/Analytics/Views/CDR/CDRLog.html', parameters, {
                useModalTemplate: true,
                width: "80%"
            });
        }
        function getFilterIds(values, idProp) {
            var filterIds = [];
            if (values.length > 0) {
                angular.forEach(values, function (val) {
                    filterIds.push(val[idProp]);
                });
            }
            return filterIds;
        }

        function getReleaseCodeMeasureEnum() {
            return utilsService.getArrayEnum(releaseCodeMeasureEnum);
        }

        function applyGroupKeysRules(parentGroupKeys, groupKeys) {
            applyZoneRule(parentGroupKeys, groupKeys);
            applyPortRule(parentGroupKeys, groupKeys);
            applyCodeBuyRule(parentGroupKeys, groupKeys);
            applyCodeSalesRule(parentGroupKeys, groupKeys);
            applySupplierZoneIdRule(parentGroupKeys, groupKeys);
            eliminateGroupKeysNotInParent(parentGroupKeys, groupKeys);
        }

        function applyZoneRule(parentGroupKeys, groupKeys) {
            for (var i = 0; i < parentGroupKeys.length; i++) {
                if (parentGroupKeys[i].value == trafficStatisticGroupKeysEnum.OurZone.value) {
                    removeCodeGroupFromGroupKeys(groupKeys);
                    removeCodeBuyFromGroupKeys(groupKeys);
                    removeCodeSalesFromGroupKeys(groupKeys);
                }

            }

        }
        function applyPortRule(parentGroupKeys, groupKeys) {
            for (var i = 0; i < parentGroupKeys.length; i++) {
                if (parentGroupKeys[i].value == trafficStatisticGroupKeysEnum.PortIn.value || parentGroupKeys[i].value == trafficStatisticGroupKeysEnum.PortOut.value) {
                    removeGateWayOutFromGroupKeys(groupKeys);
                    removeGateWayInFromGroupKeys(groupKeys);
                }

            }

        }
        function applyCodeBuyRule(parentGroupKeys, groupKeys) {
            for (var i = 0; i < parentGroupKeys.length; i++) {
                if (parentGroupKeys[i].value == trafficStatisticGroupKeysEnum.CodeGroup.value)
                    return;
            }
            removeCodeBuyFromGroupKeys(groupKeys);

        }
        function applyCodeSalesRule(parentGroupKeys, groupKeys) {
            for (var i = 0; i < parentGroupKeys.length; i++) {
                if (parentGroupKeys[i].value == trafficStatisticGroupKeysEnum.CodeGroup.value)
                    return;
            }
            removeCodeSalesFromGroupKeys(groupKeys);

        }
        function applySupplierZoneIdRule(parentGroupKeys, groupKeys) {
            for (var i = 0; i <parentGroupKeys.length; i++) {
                if (parentGroupKeys[i].value == trafficStatisticGroupKeysEnum.SupplierId.value)
                    return;
            }
            removeSupplierIdFromGroupKeys(groupKeys);

        }

        function removeGateWayOutFromGroupKeys(groupKeys) {
            for (var i = 0; i < groupKeys.length; i++) {
                if (groupKeys[i].value == trafficStatisticGroupKeysEnum.GateWayOut.value) {
                    groupKeys.splice(i, 1);
                }
            }
        }
        function removeGateWayInFromGroupKeys(groupKeys) {
            for (var i = 0; i < groupKeys.length; i++) {
                if (groupKeys[i].value == trafficStatisticGroupKeysEnum.GateWayIn.value) {
                    groupKeys.splice(i, 1);
                }
            }
        }
        function removeCodeGroupFromGroupKeys(groupKeys) {
            for (var i = 0; i < groupKeys.length; i++) {
                if (groupKeys[i].value == trafficStatisticGroupKeysEnum.CodeGroup.value) {
                    groupKeys.splice(i, 1);
                }
            }
        }
        function removeCodeSalesFromGroupKeys(groupKeys) {
            for (var i = 0; i <groupKeys.length; i++) {
                if (groupKeys[i].value == trafficStatisticGroupKeysEnum.CodeSales.value) {
                    groupKeys.splice(i, 1);
                }
            }
        }
        function removeCodeBuyFromGroupKeys(groupKeys) {
            for (var i = 0; i < groupKeys.length; i++) {
                if (groupKeys[i].value == trafficStatisticGroupKeysEnum.CodeBuy.value) {
                    groupKeys.splice(i, 1);
                }
            }
        }
        function removeSupplierIdFromGroupKeys(groupKeys) {
            for (var i = 0; i < groupKeys.length; i++) {
                if (groupKeys[i].value == trafficStatisticGroupKeysEnum.SupplierZoneId.value) {
                   groupKeys.splice(i, 1);
                }
            }
        }

        function eliminateGroupKeysNotInParent(parentGroupKeys, groupKeys) {

            for (var i = 0; i < parentGroupKeys.length; i++) {
                for (var j = 0; j < groupKeys.length; j++)
                    if (parentGroupKeys[i].value == groupKeys[j].value)
                        groupKeys.splice(j, 1);
            }
        }

        return ({
            getGenericAnalyticGroupKeys: getGenericAnalyticGroupKeys,
            getTrafficStatisticGroupKeys: getTrafficStatisticGroupKeys,
            getDefaultTrafficStatisticGroupKeys: getDefaultTrafficStatisticGroupKeys,
            getPeriods: getPeriods,
            showCdrLogModal: showCdrLogModal,
            getReleaseCodeMeasureEnum: getReleaseCodeMeasureEnum,
            getFilterIds: getFilterIds,
            applyGroupKeysRules: applyGroupKeysRules

        });


    }

    appControllers.service('AnalyticsService', analyticsServiceObj);


})(appControllers);

