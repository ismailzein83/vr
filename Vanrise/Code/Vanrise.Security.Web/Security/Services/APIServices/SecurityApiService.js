app.service('SecurityAPIService', function (BaseAPIService) {

    return ({
        Authenticate: Authenticate
    });

    function Authenticate(credentialsObject) {
        return BaseAPIService.post("/api/Security/Authenticate", credentialsObject);
    }
});