app.service("SuspiciousCaseAPIService", function (BaseAPIService) {

    return ({
        GetAllStrategies: GetAllStrategies
    });

    function GetAllStrategies() {
        return BaseAPIService.get("/api/SuspiciousCase/GetAllStrategies");
    }
});
