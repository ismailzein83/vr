
app.service('BIVisualElementService', function (VR_BI_BIAPIService) {

    return ({
        retrieveWidgetData: retrieveWidgetData,
        exportWidgetData: exportWidgetData
    });

    function retrieveWidgetData(visualElementController, visualElementSettings, filter) {
        var fromDate = new Date(filter.fromDate.getTime() + filter.fromDate.getTimezoneOffset() * 60 * 1000);
        var toDate = new Date(filter.toDate.getTime() + filter.toDate.getTimezoneOffset() * 60 * 1000);
        if(visualElementSettings.OperationType != undefined)
        switch (visualElementSettings.OperationType) {
            case "TopEntities":
                visualElementController.isTopEntities = true;
                return VR_BI_BIAPIService.GetTopEntities(visualElementSettings.EntityType, visualElementSettings.TopMeasure, fromDate, toDate, visualElementSettings.TopRecords,visualElementSettings.TimeEntity, visualElementSettings.MeasureTypes);
            case "MeasuresGroupedByTime":
                visualElementController.isDateTimeGroupedData = true;
                return VR_BI_BIAPIService.GetMeasureValues(filter.timeDimensionType.value, fromDate, toDate,visualElementSettings.TimeEntity, visualElementSettings.MeasureTypes);
                break;

        }


    }
    function exportWidgetData(visualElementController, visualElementSettings, filter) {
        var fromDate = new Date(filter.fromDate.getTime() + filter.fromDate.getTimezoneOffset() * 60 * 1000);
        var toDate = new Date(filter.toDate.getTime() + filter.toDate.getTimezoneOffset() * 60 * 1000);

        if (visualElementSettings.OperationType != undefined)
            switch (visualElementSettings.OperationType) {
                case "TopEntities":
                    visualElementController.isTopEntities = true;
                    return VR_BI_BIAPIService.ExportTopEntities(visualElementSettings.EntityType, visualElementSettings.TopMeasure, fromDate, toDate, visualElementSettings.TopRecords, visualElementSettings.TimeEntity,visualElementSettings.MeasureTypes);
                case "MeasuresGroupedByTime":
                    visualElementController.isDateTimeGroupedData = true;
                    return VR_BI_BIAPIService.ExportMeasureValues(filter.timeDimensionType.value, fromDate, toDate,visualElementSettings.TimeEntity, visualElementSettings.MeasureTypes);
                    break;

            }


    }

});