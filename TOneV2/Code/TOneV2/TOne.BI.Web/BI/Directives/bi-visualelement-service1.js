
app.service('BIVisualElementService', function (BIAPIService) {

    return ({
        retrieveWidgetData: retrieveWidgetData
    });

    function retrieveWidgetData(visualElementController, visualElementSettings, filter) {
    
        if(visualElementSettings.OperationType!=undefined)
        switch (visualElementSettings.OperationType) {
            case "TopEntities":
                visualElementController.isTopEntities = true;
                return BIAPIService.GetTopEntities(visualElementSettings.EntityType, visualElementSettings.TopMeasure, filter.fromDate, filter.toDate, 10, visualElementSettings.MeasureTypes);
            case "MeasuresGroupedByTime":
                visualElementController.isDateTimeGroupedData = true;
                return BIAPIService.GetMeasureValues(filter.timeDimensionType.value, filter.fromDate, filter.toDate, visualElementSettings.MeasureTypes);
                break;

        }


    }

});