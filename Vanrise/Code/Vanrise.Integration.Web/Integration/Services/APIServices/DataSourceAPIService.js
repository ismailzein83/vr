app.service('DataSourceAPIService', function (BaseAPIService) {

    return ({
        GetDataSources: GetDataSources
    });

    function GetDataSources() {
        return BaseAPIService.get("/api/DataSource/GetDataSources");
    }
});