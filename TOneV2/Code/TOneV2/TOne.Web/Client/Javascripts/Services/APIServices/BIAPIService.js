
app.service('BIAPIService', function (BaseAPIService) {

    return ({
        GetProfit: GetProfit,
        GetTopZonesByDuration: GetTopZonesByDuration,
        GetTopEntities: GetTopEntities,
        GetEntityMeasureValues: GetEntityMeasureValues,
        GetEntityMeasuresValues: GetEntityMeasuresValues
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
    
    function GetEntityMeasureValues(entityType, entityValue, measureType, timeDimensionType, fromDate, toDate) {
        return BaseAPIService.get("/api/BI/GetEntityMeasureValues",
            {
                entityType: entityType,
                entityValue: entityValue,
                measureType: measureType,
                timeDimensionType: timeDimensionType,
                fromDate: fromDate,
                toDate: toDate
            });
    }

    function GetEntityMeasuresValues(entityType, entityId, timeDimensionType, fromDate, toDate, measureTypes) {
        return BaseAPIService.get("/api/BI/GetEntityMeasuresValues",
            {
                entityType: entityType,
                entityId: entityId,
                timeDimensionType: timeDimensionType,
                fromDate: fromDate,
                toDate: toDate,
                measureTypes: measureTypes
            });
    }

});