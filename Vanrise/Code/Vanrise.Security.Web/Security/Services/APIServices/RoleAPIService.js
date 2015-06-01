app.service('RoleAPIService', function (BaseAPIService) {

    return ({
        GetRoles: GetRoles
    });

    function GetRoles() {
        return BaseAPIService.get("/api/Roles/GetRoles",
            {
                
            }
           );
    }

});