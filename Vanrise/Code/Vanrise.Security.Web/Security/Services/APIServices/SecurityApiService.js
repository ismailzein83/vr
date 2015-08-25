app.service('SecurityAPIService', function (BaseAPIService) {

    return ({
        Authenticate: Authenticate,
        ChangePassword: ChangePassword
    });

    function Authenticate(credentialsObject) {
        return BaseAPIService.post("/api/Security/Authenticate", credentialsObject);
    }

    function ChangePassword(ChangedPasswordObject) {
        return BaseAPIService.post("/api/Security/ChangePassword", ChangedPasswordObject);
    }
});