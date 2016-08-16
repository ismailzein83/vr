(function (appControllers) {

    'use strict';

    RatePlanUtilsService.$inject = [];

    function RatePlanUtilsService()
    {
        return {
            onNewRateChanged: onNewRateChanged
        };

        function onNewRateChanged(dataItem, settings)
        {
            if (dataItem.NewRate) // This check is false when newRate is undefined, null or an empty string
            {
                dataItem.CurrentRateNewEED = (dataItem.CurrentRateEED != null) ? dataItem.CurrentRateEED : dataItem.ZoneEED;

                dataItem.NewRateBED = (dataItem.CurrentRate == null || Number(dataItem.NewRate) > dataItem.CurrentRate) ?
                    getNowPlusDays(settings.increasedRateDayOffset) : getNowPlusDays(settings.decreasedRateDayOffset);
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
    }

    appControllers.service('WhS_Sales_RatePlanUtilsService', RatePlanUtilsService);

})(appControllers);