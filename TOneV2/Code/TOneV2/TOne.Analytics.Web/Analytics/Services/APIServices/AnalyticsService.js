(function (appControllers) {

    "use strict";

    analyticsServiceObj.$inject = ['TrafficStatisticGroupKeysEnum', 'GenericAnalyticGroupKeysEnum', 'PeriodEnum', 'UtilsService', 'VRModalService', 'ReleaseCodeMeasureEnum'];

    function analyticsServiceObj(trafficStatisticGroupKeysEnum, genericAnalyticGroupKeysEnum, periodEnum, utilsService, vrModalService, releaseCodeMeasureEnum) {
        
        function updateParametersFromGroupKeys(parameters, scope, dataItem, viewScope) {
            var groupKeys ;
            if (scope === undefined)
                return;
            if (scope === viewScope) {
                groupKeys = scope.selectedGroupKeys;
            }
            else {
                groupKeys = [scope.selectedGroupKey];
            }

            for (var i = 0; i < groupKeys.length; i++) {
                var groupKey = groupKeys[i];
                switch (groupKey.value) {
                    case trafficStatisticGroupKeysEnum.OurZone.value:
                        parameters.zoneIds.push(dataItem.GroupKeyValues[i].Id);
                        break;
                    case trafficStatisticGroupKeysEnum.CustomerId.value:
                        parameters.customerIds.push(dataItem.GroupKeyValues[i].Id);
                        break;
                    case trafficStatisticGroupKeysEnum.SupplierId.value:
                        parameters.supplierIds.push(dataItem.GroupKeyValues[i].Id);
                        break;
                    case trafficStatisticGroupKeysEnum.Switch.value:
                        parameters.switchIds.push(dataItem.GroupKeyValues[i].Id);
                        break;
                }
            }

            updateParametersFromGroupKeys(parameters, scope.gridParentScope, scope.dataItem);
        }

        function checkExpandableRow(selectedTabGroupKey,groupKeys) {
            if (groupKeys.length === 2 && selectedGroupKey.value === trafficStatisticGroupKeysEnum.OurZone.value && (groupKeys[0].value ===
            trafficStatisticGroupKeysEnum.CodeGroup.value || groupKeys.value === trafficStatisticGroupKeysEnum.CodeGroup.value))
                return false;
            else if (groupKeys.length > 1)
                return true;
            else if (selectedTabGroupKey.value === trafficStatisticGroupKeysEnum.SupplierId.value)
                return true;
            else
                return false;
        }

        function showCdrLog(parameters) {
            vrModalService.showModal('/Client/Modules/Analytics/Views/CDR/CDRLog.html', parameters, {
                useModalTemplate: true,
                width: "80%",
                maxHeight: "800px",
                title: "CDR Log"
            });
        }

        function getSubGridMenuAction(scope) {
            return [{
                name: "CDRs",
                clicked: function (dataItem) {
                    var parameters = {
                        fromDate:scope.viewScope.fromDate,
                        toDate: scope.viewScope.toDate,
                        customerIds: [],
                        zoneIds: [],
                        supplierIds: [],
                        switchIds: []
                    };
                    updateParametersFromGroupKeys(parameters, scope, dataItem,scope.viewScope);
                    showCdrLog(parameters);
                }
            }];
        }
        
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
            for (var prop in genericAnalyticGroupKeysEnum) {
                    groupKeys.push(genericAnalyticGroupKeysEnum[prop]);
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
            showCdrLog(parameters);
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
            applyGroupKeysRules: applyGroupKeysRules,
            getSubGridMenuAction: getSubGridMenuAction,
            checkExpandableRow: checkExpandableRow
        });


    }

    appControllers.service('AnalyticsService', analyticsServiceObj);


})(appControllers);

