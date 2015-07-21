 app.service('BIAPIService', function (BaseAPIService) {

        return ({
            GetMeasureValues: GetMeasureValues,
            GetEntityMeasuresValues: GetEntityMeasuresValues,
            GetTopEntities: GetTopEntities,
            GetMeasureValues1: GetMeasureValues1
    
        });


        function GetMeasureValues(timeDimensionType, fromDate, toDate, measureTypesNames) {
            return BaseAPIService.get("/api/BI/GetMeasureValues",
                {
                    timeDimensionType: timeDimensionType,
                    fromDate: fromDate,
                    toDate: toDate,
                    measureTypesNames: measureTypesNames
                });
        }

        function GetEntityMeasuresValues(entityTypeName, entityId, timeDimensionType, fromDate, toDate, measureTypesNames) {
            return BaseAPIService.get("/api/BI/GetEntityMeasuresValues",
                {
                    entityTypeName: entityTypeName,
                    entityId: entityId,
                    timeDimensionType: timeDimensionType,
                    fromDate: fromDate,
                    toDate: toDate,
                    measureTypesNames: measureTypesNames
                });
        }

        function GetTopEntities(entityTypeName, topByMeasureTypeName, fromDate, toDate, topCount, measureTypesNames) {
            return BaseAPIService.get("/api/BI/GetTopEntities",
                {
                    entityTypeName: entityTypeName,
                    topByMeasureTypeName: topByMeasureTypeName,
                    fromDate: fromDate,
                    toDate: toDate,
                    topCount: topCount,
                    measureTypesNames: measureTypesNames
                });
        }
        function GetMeasureValues1(fromDate, toDate, measureTypesNames) {
            return BaseAPIService.get("/api/BI/GetMeasureValues",
                {
                    fromDate: fromDate,
                    toDate: toDate,
                    measureTypesNames: measureTypesNames
                });
        }
    

    });