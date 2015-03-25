
app.service('BIAPIService', function (BaseAPIService) {

    return ({
        GetProfit: GetProfit
    });

    //function GetProfit(fromDate, toDate) {
    //    return BaseAPIService.get("/api/BI/GetProfit",
    //        {
    //            fromDate: fromDate,
    //            toDate: toDate
    //        }
    //       );

    function GetProfit(timeDimensionType, fromDate, toDate) {       
        return BaseAPIService.get("/api/BI/GetProfit",
            {
                timeDimensionType: timeDimensionType,
                fromDate: fromDate,
                toDate: toDate
            });
    }
});