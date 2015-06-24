


app.service('BIVisualElementService1', function (BIDataAPIService) {

    return ({
        retrieveData1: retrieveData1
    });

    function retrieveData1(visualElementController, visualElementSettings) {
        //var measureTypeValues = [];
        
        //angular.forEach(visualElementSettings.measureTypes, function (measureType) {
        //    measureTypeValues.push(measureType.Name);
        //});
        //console.log(measureTypeValues);
        switch (visualElementSettings.OperationType) {
            case "TopEntities":
                visualElementController.isTopEntities = true;
                return BIDataAPIService.GetTopEntities(visualElementSettings.EntityType, visualElementSettings.MeasureTypes[0], visualElementSettings.fromdate, visualElementSettings.todate, 10, visualElementSettings.MeasureTypes);
            case "MeasuresGroupedByTime":
                visualElementController.isDateTimeGroupedData = true;
                return BIDataAPIService.GetMeasureValues(visualElementSettings.timedimensiontype.value, visualElementSettings.fromdate, visualElementSettings.todate, visualElementSettings.MeasureTypes);
                break;
        }
    }

});