


app.service('BIVisualElementService', function (BIAPIService) {

    return ({
        retrieveData: retrieveData
    });

    function retrieveData(visualElementController, visualElementSettings) {
        var measureTypeValues = [];
        angular.forEach(visualElementSettings.measureTypes, function (measureType) {
            measureTypeValues.push(measureType.value);
        });
        switch (visualElementSettings.operationType) {
            case "TopEntities":
                visualElementController.isTopEntities = true;
                return BIAPIService.GetTopEntities(visualElementSettings.entityType.value, measureTypeValues[0], visualElementController.fromdate, visualElementController.todate, 10, measureTypeValues);
            case "MeasuresGroupedByTime":
                visualElementController.isDateTimeGroupedData = true;
                return BIAPIService.GetMeasureValues(visualElementController.timedimensiontype.value, visualElementController.fromdate, visualElementController.todate, measureTypeValues);
                break;
        }
    }

});