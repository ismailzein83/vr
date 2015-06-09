app.service('PermissionAPIService', function (BaseAPIService) {

    return ({
        GetEntityNodes: GetEntityNodes,
        GetPermissions: GetPermissions,
        UpdatePermissions: UpdatePermissions
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

    function UpdatePermissions(permissionObject) {
        return BaseAPIService.post("/api/Permission/UpdatePermissions",
             permissionObject);
    }
});