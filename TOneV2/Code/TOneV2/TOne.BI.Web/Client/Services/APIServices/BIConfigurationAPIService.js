
app.service('BIConfigurationAPIService', function (BaseAPIService) {

    return ({
        GetMeasures: GetMeasures,
        GetEntities: GetEntities,
        GetMeasureValues: GetMeasureValues,
        GetTopEntities: GetTopEntities,
    });
    function GetMeasures() {
        return BaseAPIService.get("/api/BIConfiguration/GetMeasures",
            {
            });
    }

    function GetEntities() {
        return BaseAPIService.get("/api/BIConfiguration/GetEntities",
            {
            });
    }

    function GetMeasureValues(timeDimensionType, fromDate, toDate, measureTypesIDs) {
        return BaseAPIService.get("/api/BIConfiguration/GetMeasureValues1",
            {
                timeDimensionType: timeDimensionType,
                fromDate: fromDate,
                toDate: toDate,
                measureTypesIDs: measureTypesIDs
            });
    }

    function GetEntityMeasuresValues(entityTypeID, entityId, timeDimensionType, fromDate, toDate, measureTypesIDs) {
        return BaseAPIService.get("/api/BIConfiguration/GetEntityMeasuresValues1",
            {
                entityTypeID: entityTypeID,
                entityId: entityId,
                timeDimensionType: timeDimensionType,
                fromDate: fromDate,
                toDate: toDate,
                measureTypesIDs: measureTypesIDs
            });
    }

    function GetTopEntities(entityTypeID, topByMeasureTypeID, fromDate, toDate, topCount, measureTypesIDs) {
   
        console.log(measureTypesIDs);
        return BaseAPIService.get("/api/BIConfiguration/GetTopEntities1",
            {
                entityTypeID: entityTypeID,
                topByMeasureTypeID: topByMeasureTypeID,
                fromDate: fromDate,
                toDate: toDate,
                topCount: topCount,
                measureTypesIDs: measureTypesIDs
            });
    }



});