
app.service('BIVisualElementService1', function (BIDataAPIService) {

    return ({
        retrieveWidgetData: retrieveWidgetData
    });

    function retrieveWidgetData(visualElementController, visualElementSettings, filter) {
    
        if(visualElementSettings.OperationType!=undefined)
        switch (visualElementSettings.OperationType) {
            case "TopEntities":
                visualElementController.isTopEntities = true;
                return BIDataAPIService.GetTopEntities(visualElementSettings.EntityType, visualElementSettings.TopMeasure, filter.fromDate, filter.toDate, 10, visualElementSettings.MeasureTypes);
            case "MeasuresGroupedByTime":
                visualElementController.isDateTimeGroupedData = true;
                return BIDataAPIService.GetMeasureValues(filter.timeDimensionType.value, filter.fromDate, filter.toDate, visualElementSettings.MeasureTypes);
                break;

        }


    }

});