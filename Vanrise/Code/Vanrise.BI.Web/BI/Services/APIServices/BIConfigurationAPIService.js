
app.service('BIConfigurationAPIService', function (BaseAPIService) {

    return ({
        GetMeasures: GetMeasures,
        GetEntities: GetEntities,
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




});