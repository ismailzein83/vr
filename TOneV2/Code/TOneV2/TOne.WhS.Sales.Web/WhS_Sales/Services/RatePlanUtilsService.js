(function (appControllers) {

    'use strict';

    RatePlanUtilsService.$inject = ['UtilsService'];

    function RatePlanUtilsService(UtilsService) {
        return {
            onNewRateBlurred: onNewRateBlurred,
            onNewRateChanged: onNewRateChanged,
            validateNewRate: validateNewRate,
            validateNewRateDates: validateNewRateDates,
            getNowPlusDays: getNowPlusDays,
            getNowMinusDays: getNowMinusDays,
            isSameNewService: isSameNewService,
            isStringEmpty: isStringEmpty
        };

        function onNewRateChanged(dataItem) {
            setRateChangeTypeIcon(dataItem);
        }
        function onNewRateBlurred(dataItem, settings) {
            formatNewRate(dataItem);
            setNewRateDates(dataItem, settings);
        }

        function setNewRateDates(dataItem, settings) {
            if (!isStringEmpty(dataItem.NewRate))
            {
                dataItem.CurrentRateNewEED = (dataItem.CurrentRateEED != null) ? dataItem.CurrentRateEED : dataItem.ZoneEED;

                var zoneBED = new Date(dataItem.ZoneBED);
                var newRateBED = (dataItem.CurrentRate == null || Number(dataItem.NewRate) > dataItem.CurrentRate) ? getNowPlusDays(settings.increasedRateDayOffset) : getNowPlusDays(settings.decreasedRateDayOffset);
                dataItem.NewRateBED = (newRateBED > zoneBED) ? newRateBED : zoneBED;
            }
            else {
                dataItem.NewRateBED = null;
                dataItem.NewRateEED = null;
            }

            if (dataItem.NewRateEED == null)
                dataItem.NewRateEED = dataItem.ZoneEED;
        }
        function getNowPlusDays(days) {
            return new Date(new Date().setDate(new Date().getDate() + days));
        }
        function getNowMinusDays(days) {
        	return new Date(new Date().setDate(new Date().getDate() - days));
        }

        function formatNewRate(dataItem) {
            if (dataItem.NewRate)
                dataItem.NewRate = Number(dataItem.NewRate);
        }

        function setRateChangeTypeIcon(dataItem) {
            if (dataItem.NewRate) // This check is false when newRate is undefined, null or an empty string
            {
                if (dataItem.CurrentRate == null) {
                    dataItem.RateChangeTypeIcon = null;
                    dataItem.RateChangeTypeIconTooltip = null;
                }
                else if (Number(dataItem.NewRate) > dataItem.CurrentRate) {
                    dataItem.RateChangeTypeIcon = 'glyphicon-arrow-up arrow-below';
                    dataItem.RateChangeTypeIconTooltip = 'Increase';
                }
                else if (Number(dataItem.NewRate) < dataItem.CurrentRate) {
                    dataItem.RateChangeTypeIcon = 'glyphicon-arrow-down arrow-above';
                    dataItem.RateChangeTypeIconTooltip = 'Decrease';
                }
                else {
                    dataItem.RateChangeTypeIcon = null;
                    dataItem.RateChangeTypeIconTooltip = null;
                }
            }
            else {
                dataItem.RateChangeTypeIcon = null;
                dataItem.RateChangeTypeIconTooltip = null;
            }
        }

        function validateNewRate(dataItem)
        {
            var newRate = Number(dataItem.NewRate);
            
            if (isNaN(newRate))
                return 'New rate must be a number';
            if (newRate <= 0)
                return 'New rate must be greater than 0';
            if (dataItem.CurrentRate != null && Number(dataItem.CurrentRate) == newRate)
                return 'New rate must be different than the current rate';

            return null;
        }
        function validateNewRateDates(dataItem) {
            var zoneBED = new Date(dataItem.ZoneBED);
            var zoneEED = (dataItem.ZoneEED != null) ? new Date(dataItem.ZoneEED) : null;

            var newRateBED = new Date(dataItem.NewRateBED);
            var newRateEED = (dataItem.NewRateEED != null) ? new Date(dataItem.NewRateEED) : null;

            if (newRateBED < zoneBED)
                return 'Min BED: ' + UtilsService.getShortDate(zoneBED);

            if (zoneEED != null && (newRateEED == null || newRateEED > zoneEED))
                return 'Max EED: ' + UtilsService.getShortDate(zoneEED);

            return UtilsService.validateDates(newRateBED, newRateEED);
        }

        function isSameNewService(newIds, oldIds) {
            if (newIds == undefined && oldIds == undefined)
                return true;
            if (newIds != undefined && oldIds != undefined) {
                if (newIds.length != oldIds.length)
                    return false;
                for (var i = 0; i < newIds.length; i++) {
                    if (!UtilsService.contains(oldIds, newIds[i]))
                        return false;
                }
                return true;
            }
            return false;
        }
        function isStringEmpty(string) {
            return (string === undefined || string === null || string === '');
        }
    }

    appControllers.service('WhS_Sales_RatePlanUtilsService', RatePlanUtilsService);

})(appControllers);