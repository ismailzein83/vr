(function(appControllers) {

    "use strict";

    analyticsServiceObj.$inject = ['TrafficStatisticGroupKeysEnum', 'PeriodEnum', 'UtilsService', 'VRModalService', 'ReleaseCodeMeasureEnum'];

    function analyticsServiceObj(trafficStatisticGroupKeysEnum, periodEnum, utilsService, vrModalService, releaseCodeMeasureEnum) {
        
        function getTrafficStatisticGroupKeys() {
            var groupKeys = [];
            for (var prop in trafficStatisticGroupKeysEnum) {
                if (trafficStatisticGroupKeysEnum[prop].isShownInGroupKey)
                    groupKeys.push(trafficStatisticGroupKeysEnum[prop]);
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

        return ({
            getTrafficStatisticGroupKeys: getTrafficStatisticGroupKeys,
            getDefaultTrafficStatisticGroupKeys: getDefaultTrafficStatisticGroupKeys,
            getPeriods: getPeriods,
            showCdrLogModal: showCdrLogModal,
            getReleaseCodeMeasureEnum: getReleaseCodeMeasureEnum,
            getFilterIds: getFilterIds
        });


    }

    appControllers.service('AnalyticsService', analyticsServiceObj);


})(appControllers);

