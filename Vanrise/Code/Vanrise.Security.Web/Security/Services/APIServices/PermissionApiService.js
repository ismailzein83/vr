app.service('PermissionAPIService', function (BaseAPIService) {

    return ({
        GetEntityNodes: GetEntityNodes,
        GetPermissions: GetPermissions,
        UpdatePermissions: UpdatePermissions,
        GetEffectivePermissions: GetEffectivePermissions
    });

    function GetEntityNodes() {
        return BaseAPIService.get("/api/Permission/GetEntityNodes");
    }

    function GetPermissions(holderType, holderId) {
        return BaseAPIService.get("/api/Permission/GetPermissions", 
            {
                holderType : holderType,
                holderId : holderId
            });
    }

    function GetEffectivePermissions(token) {
        return BaseAPIService.get("/api/Permission/GetEffectivePermissions",
            {
                token: token
            });
    }

    function UpdatePermissions(permissionObject) {
        return BaseAPIService.post("/api/Permission/UpdatePermissions",
             permissionObject);
    }
});