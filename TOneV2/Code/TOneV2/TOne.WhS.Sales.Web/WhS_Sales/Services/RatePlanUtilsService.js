(function (appControllers) {

    'use strict';

    RatePlanUtilsService.$inject = ['UtilsService', 'VRValidationService', 'VRDateTimeService'];

    function RatePlanUtilsService(UtilsService, VRValidationService, VRDateTimeService) {
        return {
            onNewRateBlurred: onNewRateBlurred,
            onNewRateChanged: onNewRateChanged,
            validateNewRate: validateNewRate,
            validateNewRateDates: validateNewRateDates,
            getNowPlusDays: getNowPlusDays,
            getNowMinusDays: getNowMinusDays,
            isSameNewService: isSameNewService,
            isStringEmpty: isStringEmpty,
            getNewRateBED: getNewRateBED,
            getMaxDate: getMaxDate
        };

        function onNewRateChanged(dataItem) {
            setRateChangeTypeIcon(dataItem);
        }
        function onNewRateBlurred(dataItem, settings) {
            if (isStringEmpty(dataItem.NewRate))
                dataItem.IsNewRateCancelling = null;
            formatNewRate(dataItem);
            setNewRateDates(dataItem, settings);
        }

        function setNewRateDates(dataItem, settings) {
            if (!isStringEmpty(dataItem.NewRate)) {
                dataItem.CurrentRateNewEED = (dataItem.CurrentRateEED != null) ? dataItem.CurrentRateEED : dataItem.ZoneEED;

                var zoneBED = UtilsService.createDateFromString(dataItem.ZoneBED);
                var countryBED = UtilsService.createDateFromString(dataItem.CountryBED);

                dataItem.NewRateBED = getNewRateBED(zoneBED, countryBED, dataItem.IsCountryNew, dataItem.CurrentRate, dataItem.NewRate, settings.newRateDayOffset, settings.increasedRateDayOffset, settings.decreasedRateDayOffset)
            }
            else {
                dataItem.NewRateBED = null;
                dataItem.NewRateEED = null;
            }

            if (dataItem.NewRateEED == null)
                dataItem.NewRateEED = dataItem.ZoneEED;
        }
        function getNowPlusDays(daysToAdd) {
            var dayOfToday = VRDateTimeService.getNowDateTime().getDate();
            var totalDays = dayOfToday + daysToAdd;
            var todayWithoutTime = UtilsService.getDateFromDateTime(VRDateTimeService.getNowDateTime());
            var todayPlusDays = todayWithoutTime.setDate(totalDays); // setDate returns the number of milliseconds between the date object and midnight January 1 1970
            return new Date(todayPlusDays);
        }
        function getNowMinusDays(days) {
            return new Date(VRDateTimeService.getNowDateTime().setDate(VRDateTimeService.getNowDateTime().getDate() - days));
        }

        function formatNewRate(dataItem) {
            if (dataItem.NewRate)
                dataItem.NewRate = Number(dataItem.NewRate);
        }

        function setRateChangeTypeIcon(dataItem) {
            if (dataItem.NewRate) // This check is false when newRate is undefined, null or an empty string
            {
                if (dataItem.IsCountryNew == true) {
                    dataItem.RateChangeTypeIcon = null;
                    dataItem.RateChangeTypeIconTooltip = null;
                    return;
                }

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

        function validateNewRate(dataItem, ownerCurrencyId) {
            var newRate = parseFloat(dataItem.NewRate);
            if (newRate <= 0)
                return 'New rate must be greater than 0';
            if (dataItem.CurrentRate != null) {
                var currentRate = parseFloat(dataItem.CurrentRate);
                if (currentRate == newRate) {
                    if (dataItem.IsCurrentRateEditable === false)
                        return null;
                    if (dataItem.IsNewRateCancelling)
                        return null;
                    if (dataItem.CurrentRateCurrencyId == ownerCurrencyId)
                        return 'New rate must be different than the current rate';
                }
            }
            return null;
        }
        function validateNewRateDates(dataItem) {

            var zoneBED = UtilsService.createDateFromString(dataItem.ZoneBED);
            var zoneEED = (dataItem.ZoneEED != null) ? UtilsService.createDateFromString(dataItem.ZoneEED) : null;
            var countryBED = (dataItem.CountryBED != null) ? UtilsService.createDateFromString(dataItem.CountryBED) : null;
            var minRateBED = zoneBED;

            if (countryBED != null && countryBED > minRateBED)
                minRateBED = countryBED;

            if (dataItem.NewRateBED < minRateBED)
                return 'Min BED: ' + UtilsService.getShortDate(minRateBED);

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

        function getNewRateBED(zoneBED, countryBED, isCountryNew, currentRateValue, newRateValue, newRateDayOffset, increasedRateDayOffset, decreasedRateDayOffset) {
            var dates = [];

            dates.push(zoneBED);
            dates.push(countryBED);

            var calculatedRateBED = (isCountryNew === true) ? countryBED :
                getCalculatedRateBED(currentRateValue, newRateValue, isCountryNew, newRateDayOffset, increasedRateDayOffset, decreasedRateDayOffset);

            dates.push(calculatedRateBED);
            return getMaxDate(dates);
        }
        function getCalculatedRateBED(currentRateValue, newRateValue, isCountryNew, newRateDayOffset, increasedRateDayOffset, decreasedRateDayOffset) {
            var dayOffset = 0;
            if (currentRateValue == undefined)
                dayOffset = newRateDayOffset;
            else if (newRateValue > currentRateValue)
                dayOffset = increasedRateDayOffset;
            else if (newRateValue < currentRateValue)
                dayOffset = decreasedRateDayOffset;
            return getNowPlusDays(dayOffset);
        }
        function getMaxDate(dates) {
            var maxDate;
            for (var i = 0; i < dates.length; i++) {
                var currentDate = dates[i];
                if (maxDate == undefined || (currentDate != undefined && currentDate > maxDate))
                    maxDate = currentDate;
            }
            return maxDate;
        }
    }

    appControllers.service('WhS_Sales_RatePlanUtilsService', RatePlanUtilsService);

})(appControllers);