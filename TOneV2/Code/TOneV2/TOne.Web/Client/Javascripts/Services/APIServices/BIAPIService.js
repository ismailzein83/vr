
app.service('BIAPIService', function (BaseAPIService) {

    return ({
        
        GetMeasureTypeList: GetMeasureTypeList,
        GetMeasureValues: GetMeasureValues,
        GetEntityMeasuresValues: GetEntityMeasuresValues,
        GetTopEntities: GetTopEntities
    });


    function GetMeasureTypeList() {
        return BaseAPIService.get("/api/BI/GetMeasureTypeList",
            {

            });
    }

    function GetMeasureValues(timeDimensionType, fromDate, toDate, measureTypes) {
        return BaseAPIService.get("/api/BI/GetMeasureValues",
            {
                timeDimensionType: timeDimensionType,
                fromDate: fromDate,
                toDate: toDate,
                measureTypes: measureTypes
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

    function GetTopEntities(entityType, topByMeasureType, fromDate, toDate, topCount, moreMeasures) {
        return BaseAPIService.get("/api/BI/GetTopEntities",
            {
                entityType: entityType,
                topByMeasureType: topByMeasureType,
                fromDate: fromDate,
                toDate: toDate,
                topCount: topCount,
                moreMeasures: moreMeasures
            });
    }


});