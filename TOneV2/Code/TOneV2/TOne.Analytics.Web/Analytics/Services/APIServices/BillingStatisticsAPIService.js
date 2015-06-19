
app.service('BillingStatisticsAPIService', function (BaseAPIService) {

    return ({

        GetVariationReport: GetVariationReport
    });

    function GetVariationReport(selectedDate, periodCount, periodTypeValue, selectedReportOption) {
        return BaseAPIService.get("/api/BillingStatistics/GetVariationReport",
            {
                selectedDate: selectedDate,
                periodCount: periodCount,
                periodTypeValue: periodTypeValue,
                variationReportOptionValue: selectedReportOption
            }
            );
    }

});