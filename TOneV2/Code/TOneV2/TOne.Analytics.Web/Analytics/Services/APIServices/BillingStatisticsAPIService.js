
app.service('BillingStatisticsAPIService', function (BaseAPIService) {

    return ({

        GetVariationReport: GetVariationReport
    });

    function GetVariationReport(selectedDate, periodCount, timePeriod, variationReportOption, fromRow, toRow,entityType,entityID,groupingBy) {
        return BaseAPIService.get("/api/BillingStatistics/GetVariationReport",
            {
                selectedDate: selectedDate,
                periodCount: periodCount,
                timePeriod: timePeriod,
                variationReportOption: variationReportOption,
                fromRow: fromRow,
                toRow: toRow,
                entityType: entityType,
                entityID: entityID,
                groupingBy: groupingBy

            }
            );
    }


});