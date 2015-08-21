app.service('SecurityAPIService', function (BaseAPIService) {

    return ({
        Authenticate: Authenticate,
        ResetPassword: ResetPassword
    });

    function Authenticate(credentialsObject) {
        return BaseAPIService.post("/api/Security/Authenticate", credentialsObject);
    }

    function ResetPassword(oldPassword, newPassword) {
        return BaseAPIService.get("/api/Security/ResetPassword", {
            oldPassword: oldPassword,
            newPassword: newPassword
        });
    }
});