(function(appControllers) {

    "use strict";

    analyticsServiceObj.$inject = ['TrafficStatisticGroupKeysEnum', 'PeriodEnum', 'UtilsService'];

    function analyticsServiceObj(trafficStatisticGroupKeysEnum, periodEnum, utilsService) {
        
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



        return ({
            getTrafficStatisticGroupKeys: getTrafficStatisticGroupKeys,
            getDefaultTrafficStatisticGroupKeys: getDefaultTrafficStatisticGroupKeys,
            getPeriods: getPeriods,
        });


    }

    appControllers.service('AnalyticsService', analyticsServiceObj);


})(appControllers);

