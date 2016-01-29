(function (appControllers) {
    
    'use strict';

    ParametersService.$inject = ['VRCommon_HourEnum', 'UtilsService'];

    function ParametersService(VRCommon_HourEnum, UtilsService) {
        return {
            getDefaultPeakHourIds: getDefaultPeakHourIds
        };

        function getDefaultPeakHourIds() {
            var deferred = UtilsService.createPromiseDeferred();
            var ids = [];
            var hours = UtilsService.getArrayEnum(VRCommon_HourEnum);
            for (var key in hours) {
                var hour = hours[key];
                if (hour.id >= 12 && hour.id <= 17) {
                    ids.push(hour.id);
                }
            }
            deferred.resolve(ids);
            return deferred.promise;
        }
    }

    appControllers.service('CDRAnalysis_FA_ParametersService', ParametersService);

})(appControllers);