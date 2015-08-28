
app.service('BIConfigurationAPIService', function (BaseAPIService) {

    return ({
        GetMeasures: GetMeasures,
        GetEntities: GetEntities,
        GetSystemCurrency: GetSystemCurrency
    });
    function GetMeasures() {
        return BaseAPIService.get("/api/BIConfiguration/GetMeasures",
            {
            });
    }

    function GetEntities() {
        return BaseAPIService.get("/api/BIConfiguration/GetEntities",
            {
            });
    }
    function GetSystemCurrency() {
        return BaseAPIService.get("/api/BIConfiguration/GetSystemCurrency");
    }




});