(function (appControllers) {

    'use strict';

    RatePlanUtilsService.$inject = ['UtilsService', 'VRValidationService'];

    function RatePlanUtilsService(UtilsService, VRValidationService) {
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

                var zoneBED = UtilsService.createDateFromString(dataItem.ZoneBED);
                var newRateBED = getNewRateBED(dataItem.CurrentRate, dataItem.NewRate, settings);
                dataItem.NewRateBED = (newRateBED > zoneBED) ? newRateBED : zoneBED;
            }
            else {
                dataItem.NewRateBED = null;
                dataItem.NewRateEED = null;
            }

            if (dataItem.NewRateEED == null)
                dataItem.NewRateEED = dataItem.ZoneEED;
        }
        function getNewRateBED(currentRate, newRate, settings) {
        	var dayOffset = 0;

        	if (currentRate == undefined) {
        		dayOffset = settings.newRateDayOffset;
        	}
        	else {
        		var currentRateAsNumber = Number(currentRate);
        		var newRateAsNumber = Number(newRate);

        		if (newRateAsNumber > currentRateAsNumber) {
        			dayOffset = settings.increasedRateDayOffset;
        		}
        		else if (newRateAsNumber < currentRateAsNumber) {
        			dayOffset = settings.decreasedRateDayOffset;
        		}
        	}

        	return getNowPlusDays(dayOffset);
        }
        function getNowPlusDays(daysToAdd) {
        	var dayOfToday = new Date().getDate();
        	var totalDays = dayOfToday + daysToAdd;
        	var todayWithoutTime = UtilsService.getDateFromDateTime(new Date());
        	var todayPlusDays = todayWithoutTime.setDate(totalDays); // setDate returns the number of milliseconds between the date object and midnight January 1 1970
        	return new Date(todayPlusDays);
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

            var zoneBED = UtilsService.createDateFromString(dataItem.ZoneBED);
            var zoneEED = (dataItem.ZoneEED != null) ? UtilsService.createDateFromString(dataItem.ZoneEED) : null;

            if (dataItem.NewRateBED < zoneBED)
                return 'Min BED: ' + UtilsService.getShortDate(zoneBED);

            if (zoneEED != null && (dataItem.NewRateEED == null || dataItem.NewRateEED > zoneEED))
                return 'Max EED: ' + UtilsService.getShortDate(zoneEED);

            return VRValidationService.validateTimeRange(dataItem.NewRateBED, dataItem.NewRateEED);
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