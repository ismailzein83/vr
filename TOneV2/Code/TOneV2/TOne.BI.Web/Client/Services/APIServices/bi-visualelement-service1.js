


app.service('BIVisualElementService1', function (BIConfigurationAPIService) {

    return ({
        retrieveData1: retrieveData1
    });

    function retrieveData1(visualElementController, visualElementSettings) {
        var measureTypeValues = [];
        
        angular.forEach(visualElementSettings.measureTypes, function (measureType) {
            measureTypeValues.push(measureType.Id);
        });

        console.log(visualElementSettings.entityType.Id);
        console.log(visualElementSettings.measureTypes[0]);
        console.log(visualElementController.fromdate);
        console.log(visualElementController.todate);
        console.log(visualElementSettings.measureTypes);
       // console.log(visualElementSettings.entityType.Id);
        console.log(visualElementSettings.operationType.value);
        switch (visualElementSettings.operationType.value) {
            case "TopEntities":
                visualElementController.isTopEntities = true;
                return BIConfigurationAPIService.GetTopEntities(visualElementSettings.entityType.Id, visualElementSettings.measureTypes[0].Id, visualElementController.fromdate, visualElementController.todate, 10, measureTypeValues);
            case "MeasuresGroupedByTime":
                visualElementController.isDateTimeGroupedData = true;
                return BIConfigurationAPIService.GetMeasureValues(visualElementController.timedimensiontype.value, visualElementController.fromdate, visualElementController.todate, measureTypeValues);
                break;
        }
    }

});