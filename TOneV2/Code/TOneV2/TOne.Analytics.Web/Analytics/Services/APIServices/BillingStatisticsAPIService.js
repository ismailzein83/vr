
app.service('BillingStatisticsAPIService', function (BaseAPIService) {

    return ({
        GetTest: GetTest,
        GetZoneProfit: GetZoneProfit,
        GetBillingStatistics: GetBillingStatistics
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


    

});