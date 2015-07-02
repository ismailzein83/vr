﻿
app.service('BIVisualElementService1', function (BIDataAPIService) {

    return ({
        retrieveData1: retrieveData1
    });

    function retrieveData1(visualElementController, visualElementSettings) {
       
        if(visualElementSettings.OperationType!=undefined)
        switch (visualElementSettings.OperationType) {
            case "TopEntities":
                visualElementController.isTopEntities = true;
                console.log(visualElementController);
                console.log(visualElementSettings);
                return BIDataAPIService.GetTopEntities(visualElementSettings.EntityType, visualElementSettings.TopMeasure, visualElementController.filter.fromDate, visualElementController.filter.toDate, 10, visualElementSettings.MeasureTypes);
            case "MeasuresGroupedByTime":
                visualElementController.isDateTimeGroupedData = true;
                return BIDataAPIService.GetMeasureValues(visualElementController.filter.timeDimensionType.value, visualElementController.filter.fromDate, visualElementController.filter.toDate, visualElementSettings.MeasureTypes);
                break;

        }


    }

});