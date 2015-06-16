app.service('RoleAPIService', function (BaseAPIService) {

    return ({
        GetFilteredRoles: GetFilteredRoles,
        GetRole: GetRole,
        GetRoles: GetRoles,
        AddRole: AddRole,
        UpdateRole: UpdateRole
    });

    function GetFilteredRoles(fromRow, toRow, name) {
        return BaseAPIService.get("/api/Roles/GetFilteredRoles",
            {
                fromRow: fromRow,
                toRow: toRow,
                name: name
            }
           );
    }

    function GetRole(roleId) {
        return BaseAPIService.get("/api/Roles/GetRole",
            {
                roleId: roleId
            }
           );
    }

    function GetRoles() {
        return BaseAPIService.get("/api/Roles/GetRoles");
    }

    function AddRole(role) {
        return BaseAPIService.post("/api/Roles/AddRole",
            role
           );
    }

    function UpdateRole(role) {
        return BaseAPIService.post("/api/Roles/UpdateRole",
           role
           );
    }
});