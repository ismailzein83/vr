
app.service('BillingStatisticsAPIService', function (BaseAPIService) {

    return ({
        GetTest: GetTest,
        GetZoneProfit: GetZoneProfit,
        GetBillingStatistics: GetBillingStatistics,
        GetVariationReportsFinalData: GetVariationReportsFinalData,
        GetVariationReportsData: GetVariationReportsData,
        GetVariationReportQuery: getVariationReportQuery
    });

    function GetTest(name){
        return BaseAPIService.get("/api/BillingStatistics/GetTest",
            {
                name: name
            }
           );
    }
        function GetZoneProfit(date1,date2) {
            return BaseAPIService.get("/api/BillingStatistics/GetZoneProfit",
            {
                date1: date1,
                date2: date2
            }
            );
        }

        function GetBillingStatistics(fromDate, toDate) {
            return BaseAPIService.get("/api/BillingStatistics/GetBillingStatistics",
            {
                fromDate: fromDate,
                toDate: toDate
            }
            );
        }

        function GetVariationReportsData(selectedDate, periodCount, periodTypeValue, variationReportOptionValue) {
                return BaseAPIService.get("/api/BillingStatistics/GetVariationReportsData",
                    {
                        selectedDate: selectedDate,
                        periodCount: periodCount,
                        periodTypeValue: periodTypeValue,
                        variationReportOptionValue: variationReportOptionValue
                    }
                    );
            }

        function GetVariationReportsFinalData(selectedDate, periodCount, periodTypeValue, variationReportOptionValue) {
            return BaseAPIService.get("/api/BillingStatistics/GetVariationReportsFinalData",
                {
                    selectedDate: selectedDate,
                    periodCount: periodCount,
                    periodTypeValue: periodTypeValue,
                    variationReportOptionValue : variationReportOptionValue
                }
                );
        }

        function getVariationReportQuery(selectedDate, periodCount, periodTypeValue, selectedReportOption) {
            return BaseAPIService.get("/api/BillingStatistics/GetVariationReportQuery",
                {   selectedDate: selectedDate,
                    periodCount: periodCount,
                    periodTypeValue: periodTypeValue, 
                    variationReportOptionValue: selectedReportOption
                }
                );
        }

});