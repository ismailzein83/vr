app.service('UserAPIService', function (BaseAPIService) {

    return ({
        GetUsers: GetUsers
    });

    function GetUsers() {
        return BaseAPIService.get("/api/User/GetUsers");
    }

});