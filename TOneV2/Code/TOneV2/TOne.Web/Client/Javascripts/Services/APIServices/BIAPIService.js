
app.service('BIAPIService', function (BaseAPIService) {

    return ({
        GetProfit: GetProfit,
        GetTopZonesByDuration: GetTopZonesByDuration,
        GetTopEntities: GetTopEntities,
        GetEntityMeasureValues: GetEntityMeasureValues,
        GetEntityMeasuresValues: GetEntityMeasuresValues,
        GetMeasureTypeList: GetMeasureTypeList
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

    function GetTopEntities(entityType, measureType, fromDate, toDate, topCount, moreMeasures) {
        return BaseAPIService.get("/api/BI/GetTopEntities",
            {
                entityType: entityType,
                measureType: measureType,
                fromDate: fromDate,
                toDate: toDate,
                topCount: topCount,
                moreMeasures: moreMeasures
            });
    }
    
    function GetEntityMeasureValues(entityType, entityId, measureType, timeDimensionType, fromDate, toDate) {
        return BaseAPIService.get("/api/BI/GetEntityMeasureValues",
            {
                entityType: entityType,
                entityId: entityId,
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

    function GetMeasureTypeList() {
        return BaseAPIService.get("/api/BI/GetMeasureTypeList",
            {
               
            });
    }

});