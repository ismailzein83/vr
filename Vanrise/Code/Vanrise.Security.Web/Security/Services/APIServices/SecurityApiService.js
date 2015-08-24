app.service('SecurityAPIService', function (BaseAPIService) {

    return ({
        Authenticate: Authenticate,
        ChangePassword: ChangePassword
    });

    function Authenticate(credentialsObject) {
        return BaseAPIService.post("/api/Security/Authenticate", credentialsObject);
    }

    function ChangePassword(oldPassword, newPassword) {
        return BaseAPIService.get("/api/Security/ChangePassword", {
            oldPassword: oldPassword,
            newPassword: newPassword
        });
    }
});