app.service('PermissionAPIService', function (BaseAPIService) {

    return ({
        GetPermissionsByHolder: GetPermissionsByHolder,
        GetPermissionsByEntity: GetPermissionsByEntity,
        UpdatePermissions: UpdatePermissions,
        DeletePermission: DeletePermission,
        GetEffectivePermissions: GetEffectivePermissions
    });

    function GetPermissionsByHolder(holderType, holderId) {
        return BaseAPIService.get("/api/Permission/GetPermissionsByHolder",
            {
                holderType : holderType,
                holderId : holderId
            });
    }

    function GetPermissionsByEntity(entityType, entityId) {
        return BaseAPIService.get("/api/Permission/GetPermissionsByEntity",
            {
                entityType: entityType,
                entityId: entityId
            });
    }

    function GetEffectivePermissions() {
        return BaseAPIService.get("/api/Permission/GetEffectivePermissions");
    }

    function DeletePermission(holderType, holderId, entityType, entityId) {
        return BaseAPIService.get("/api/Permission/DeletePermission",
            {
                holderType : holderType,
                holderId : holderId,
                entityType: entityType,
                entityId: entityId
            });
    }

    function UpdatePermissions(permissionsArray) {
        return BaseAPIService.post("/api/Permission/UpdatePermissions",
             permissionsArray);
    }
});