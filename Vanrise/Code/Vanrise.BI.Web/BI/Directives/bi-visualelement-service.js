'use strict';
app.service('BIVisualElementService', function (VR_BI_BIAPIService) {

    return ({
        retrieveWidgetData: retrieveWidgetData,
        exportWidgetData: exportWidgetData
    });

    function retrieveWidgetData(visualElementController, visualElementSettings, filter) {
        if(visualElementSettings.OperationType != undefined)
        switch (visualElementSettings.OperationType) {
            case "TopEntities":
                visualElementController.isTopEntities = true;
                return VR_BI_BIAPIService.GetTopEntities(getInputObject(visualElementSettings.OperationType, visualElementSettings, filter));
            case "MeasuresGroupedByTime":
                visualElementController.isDateTimeGroupedData = true;
                return VR_BI_BIAPIService.GetMeasureValues(getInputObject(visualElementSettings.OperationType, visualElementSettings, filter));
                break;

        }


    }

    function exportWidgetData(visualElementController, visualElementSettings, filter) {

        if (visualElementSettings.OperationType != undefined)
            switch (visualElementSettings.OperationType) {
                case "TopEntities":
                    visualElementController.isTopEntities = true;
                    return VR_BI_BIAPIService.ExportTopEntities(getInputObject(visualElementSettings.OperationType, visualElementSettings, filter));
                case "MeasuresGroupedByTime":
                    visualElementController.isDateTimeGroupedData = true;
                    return VR_BI_BIAPIService.ExportMeasureValues(getInputObject(visualElementSettings.OperationType, visualElementSettings, filter));
                    break;

            }


    }

    function getInputObject(operationType, visualElementSettings, filter)
    {
        var fromDate;
        var toDate;
        if (filter.fromDate != undefined)
            fromDate = new Date(filter.fromDate.getTime() + filter.fromDate.getTimezoneOffset() * 60 * 1000);
        if (filter.toDate != undefined)
            toDate = new Date(filter.toDate.getTime() + filter.toDate.getTimezoneOffset() * 60 * 1000);
        switch (operationType) {
            case "TopEntities":
                var input = {
                    EntityTypeName: visualElementSettings.EntityType,
                    TopByMeasureTypeName: visualElementSettings.TopMeasure,
                    FromDate: fromDate,
                    ToDate: toDate,
                    TopCount: visualElementSettings.TopRecords,
                    TimeEntityName: visualElementSettings.TimeEntity,
                    MeasureTypesNames: visualElementSettings.MeasureTypes,
                    Filter: visualElementSettings.Filter
                };
                return input;
            case "MeasuresGroupedByTime":
                var input = {
                    TimeDimensionType: filter.timeDimensionType.value,
                    FromDate: fromDate,
                    ToDate: toDate,
                    TimeEntityName: visualElementSettings.TimeEntity,
                    MeasureTypesNames: visualElementSettings.MeasureTypes
                };
                return input;
        }
        
    }

});