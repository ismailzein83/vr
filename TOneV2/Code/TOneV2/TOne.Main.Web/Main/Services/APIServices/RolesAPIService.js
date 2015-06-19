app.service('RolesAPIService', function (BaseAPIService) {

    return ({
        GetRoleList: GetRoleList,
        DeleteRole: DeleteRole,
        AddRole: AddRole,
        UpdateRole: UpdateRole,
        SearchRole: SearchRole
    });

    function GetRoleList(params) {
        return BaseAPIService.get("/api/Role/GetRoles",
            {
                fromRow: params.fromRow,
                toRow: params.toRow
            }
           );
    }

    function DeleteRole(id) {
        return BaseAPIService.get("/api/Role/DeleteRole",
            {
                Id: id
            }
           );
    }

    function AddRole(Role) {
        return BaseAPIService.post("/api/Role/AddRole",
            {
                RoleId: Role.RoleId,
                Name: Role.Name,
                IsActive: Role.IsActive,
                Description: Role.Description
            }
           );
    }

    function UpdateRole(Role) {
        return BaseAPIService.post("/api/Role/UpdateRole",
            {
                RoleId: Role.RoleId,
                Name: Role.Name,
                IsActive: Role.IsActive,
                Description: Role.Description
            }
           );
    }

    function SearchRole(name) {
        return BaseAPIService.get("/api/Role/SearchRole",
            {
                Name: name
            }
           );
    }

});