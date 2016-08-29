(function (appControllers) {

    'use strict';

    RatePlanUtilsService.$inject = ['UtilsService', 'WhS_BE_RateChangeTypeEnum'];

    function RatePlanUtilsService(UtilsService, WhS_BE_RateChangeTypeEnum)
    {
        return {
            onNewRateChanged: onNewRateChanged,
            validateNewRate:validateNewRate,
            validateNewRateDates: validateNewRateDates
        };

        function onNewRateChanged(dataItem, settings, shouldSetNewRateDates)
        {
            if (shouldSetNewRateDates === true)
                setNewRateDates(dataItem, settings);
            setRateChangeTypeIcon(dataItem);
        }
        function setNewRateDates(dataItem, settings)
        {
            if (dataItem.NewRate) // This check is false when newRate is undefined, null or an empty string
            {
                dataItem.CurrentRateNewEED = (dataItem.CurrentRateEED != null) ? dataItem.CurrentRateEED : dataItem.ZoneEED;

                dataItem.NewRateBED = (dataItem.CurrentRate == null || Number(dataItem.NewRate) > dataItem.CurrentRate) ?
                    getNowPlusDays(settings.increasedRateDayOffset) :
                    getNowPlusDays(settings.decreasedRateDayOffset);
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
        function setRateChangeTypeIcon(dataItem)
        {
            if (dataItem.NewRate) // This check is false when newRate is undefined, null or an empty string
            {
                if (dataItem.CurrentRate == null) {
                    dataItem.RateChangeTypeIcon = null;
                }
                else if (Number(dataItem.NewRate) > dataItem.CurrentRate) {
                    dataItem.RateChangeTypeIcon = 'glyphicon-arrow-up arrow-above';
                }
                else if (Number(dataItem.NewRate) < dataItem.CurrentRate) {
                    dataItem.RateChangeTypeIcon = 'glyphicon-arrow-down arrow-below';
                }
                else {
                    dataItem.RateChangeTypeIcon = null;
                }
            }
            else {
                dataItem.RateChangeTypeIcon = null;
            }
        }

        function validateNewRate(dataItem) {
            if (dataItem.CurrentRate != null && Number(dataItem.CurrentRate) == Number(dataItem.NewRate))
                return 'New Rate = Current Rate';
            return null;
        }
        function validateNewRateDates(dataItem)
        {
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
    }

    appControllers.service('WhS_Sales_RatePlanUtilsService', RatePlanUtilsService);

})(appControllers);