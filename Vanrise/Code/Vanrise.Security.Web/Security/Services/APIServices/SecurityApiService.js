app.service('SecurityAPIService', function (BaseAPIService) {

    return ({
        Authenticate: Authenticate
    });

    function Authenticate() {
        return BaseAPIService.get("/api/Security/Authenticate");
    }
});