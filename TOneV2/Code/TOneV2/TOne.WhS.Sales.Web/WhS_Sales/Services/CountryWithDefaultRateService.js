app.service('WhS_Sales_CountryWithDefaultRateService', [function () {
    var drillDownDefinitions = [];

    return {
        getDrillDownDefinition: getDrillDownDefinition
    };

    function getDrillDownDefinition() {
        return drillDownDefinitions;
    }
}]);