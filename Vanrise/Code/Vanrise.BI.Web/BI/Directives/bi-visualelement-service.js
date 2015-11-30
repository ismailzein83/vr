
app.service('BIVisualElementService', function (BIAPIService) {

    return ({
        retrieveWidgetData: retrieveWidgetData,
        exportWidgetData: exportWidgetData
    });

    function retrieveWidgetData(visualElementController, visualElementSettings, filter) {
        var fromDate = new Date(filter.fromDate.getTime() + filter.fromDate.getTimezoneOffset() * 60 * 1000);
        var toDate = new Date(filter.toDate.getTime() + filter.toDate.getTimezoneOffset() * 60 * 1000);
        if(visualElementSettings.OperationType!=undefined)
        switch (visualElementSettings.OperationType) {
            case "TopEntities":
                visualElementController.isTopEntities = true;
                return BIAPIService.GetTopEntities(visualElementSettings.EntityType, visualElementSettings.TopMeasure, fromDate, toDate, visualElementSettings.TopRecords, visualElementSettings.MeasureTypes);
            case "MeasuresGroupedByTime":
                visualElementController.isDateTimeGroupedData = true;
                return BIAPIService.GetMeasureValues(filter.timeDimensionType.value, fromDate, toDate, visualElementSettings.MeasureTypes);
                break;

        }


    }
    function exportWidgetData(visualElementController, visualElementSettings, filter) {
        if (visualElementSettings.OperationType != undefined)
            switch (visualElementSettings.OperationType) {
                case "TopEntities":
                    visualElementController.isTopEntities = true;
                    return BIAPIService.ExportTopEntities(visualElementSettings.EntityType, visualElementSettings.TopMeasure, filter.fromDate, filter.toDate, 10, visualElementSettings.MeasureTypes);
                case "MeasuresGroupedByTime":
                    visualElementController.isDateTimeGroupedData = true;
                    return BIAPIService.ExportMeasureValues(filter.timeDimensionType.value, filter.fromDate, filter.toDate, visualElementSettings.MeasureTypes);
                    break;

            }


    }

});