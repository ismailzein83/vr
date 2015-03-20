
app.service('AnalyticsAPIService', function (BaseAPIService) {

    return ({
        GetTopNDestinations: GetTopNDestinations
    });

    function GetTopNDestinations(fromDate, toDate, from, to, topCount, showSupplier, groupByCodeGroup, codeGroup) {
        return BaseAPIService.get("/api/Analytics/GetTopNDestinations",
            {
                fromDate: fromDate,
                toDate: toDate,
                from: from,
                to: 20,
                topCount: 20,
                showSupplier: showSupplier,
                groupByCodeGroup: groupByCodeGroup,
                codeGroup: codeGroup
            }
           );
    }

});