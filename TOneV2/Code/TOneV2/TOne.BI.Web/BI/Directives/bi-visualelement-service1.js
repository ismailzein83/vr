


app.service('BIVisualElementService1', function (BIDataAPIService) {

    return ({
        retrieveData1: retrieveData1
    });

    function retrieveData1(visualElementController, visualElementSettings) {
        var measureTypeValues = [];
        
        angular.forEach(visualElementSettings.measureTypes, function (measureType) {
            measureTypeValues.push(measureType.Name);
        });
        console.log(measureTypeValues);
        switch (visualElementSettings.operationType.value) {
            case "TopEntities":
                visualElementController.isTopEntities = true;
                return BIDataAPIService.GetTopEntities(visualElementSettings.entityType.Name, visualElementSettings.measureTypes[0].Name, visualElementSettings.fromdate, visualElementSettings.todate, 10, measureTypeValues);
            case "MeasuresGroupedByTime":
                visualElementController.isDateTimeGroupedData = true;
                return BIDataAPIService.GetMeasureValues(visualElementSettings.timedimensiontype.value, visualElementSettings.fromdate, visualElementSettings.todate, measureTypeValues);
                break;
        }
    }

});