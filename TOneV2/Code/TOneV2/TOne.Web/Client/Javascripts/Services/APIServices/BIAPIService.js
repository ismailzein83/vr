
app.service('BIAPIService', function (BaseAPIService) {

    return ({
        GetProfit: GetProfit,
        GetTopZonesByDuration: GetTopZonesByDuration,
        GetTopEntities: GetTopEntities
    });

    function GetProfit(timeDimensionType, fromDate, toDate) {       
        return BaseAPIService.get("/api/BI/GetProfit",
            {
                timeDimensionType: timeDimensionType,
                fromDate: fromDate,
                toDate: toDate
            });
    }

    function GetTopZonesByDuration(timeDimensionType, fromDate, toDate, topCount) {
        return BaseAPIService.get("/api/BI/GetTopZonesByDuration",
            {
                timeDimensionType: timeDimensionType,
                fromDate: fromDate,
                toDate: toDate,
                topCount: topCount
            });
    }

    function GetTopEntities(entityType, measureType, fromDate, toDate, topCount) {
        return BaseAPIService.get("/api/BI/GetTopEntities",
            {
                entityType: entityType,
                measureType: measureType,
                fromDate: fromDate,
                toDate: toDate,
                topCount: topCount
            });
    }

});