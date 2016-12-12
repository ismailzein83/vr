(function (appControllers) {

    'use strict';

    PermissionAPIService.$inject = ['BaseAPIService', 'VR_Sec_ModuleConfig', 'UtilsService', 'SecurityService'];

    function PermissionAPIService(BaseAPIService, VR_Sec_ModuleConfig, UtilsService, SecurityService) {
        var controllerName = 'Permission';

        return ({
            GetFilteredEntityPermissions: GetFilteredEntityPermissions,
            GetHolderPermissions: GetHolderPermissions,
            GetEffectivePermissions: GetEffectivePermissions,
            UpdatePermissions: UpdatePermissions,
            DeletePermissions: DeletePermissions,
            HasUpdatePermissionsPermission: HasUpdatePermissionsPermission,
            HasDeleteSystemEntitiesPermission: HasDeleteSystemEntitiesPermission,
            HasDisablePermission: HasDisablePermission,
            HasEnablePermission: HasEnablePermission
        });

        function GetFilteredEntityPermissions(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, controllerName, 'GetFilteredEntityPermissions'), input);
        }

        function GetHolderPermissions(holderType, holderId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, controllerName, 'GetHolderPermissions'), {
                holderType: holderType,
                holderId: holderId
            });
        }

        function GetEffectivePermissions() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, controllerName, 'GetEffectivePermissions'));
        }

        function UpdatePermissions(permissionsArray) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, controllerName, 'UpdatePermissions'), permissionsArray);
        }

        function DeletePermissions(holderType, holderId, entityType, entityId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, controllerName, 'DeletePermissions'), {
                holderType: holderType,
                holderId: holderId,
                entityType: entityType,
                entityId: entityId
            });
        }


        function HasUpdatePermissionsPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(VR_Sec_ModuleConfig.moduleName, controllerName, ['UpdatePermissions']));
        }

        function HasDeleteSystemEntitiesPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(VR_Sec_ModuleConfig.moduleName, controllerName, ['DeletePermissions']));
        }

        function HasDisablePermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(VR_Sec_ModuleConfig.moduleName, controllerName, ['DisablePermissions']));
        }

        function HasEnablePermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(VR_Sec_ModuleConfig.moduleName, controllerName, ['EnablePermissions']));
        }
    }

    appControllers.service('VR_Sec_PermissionAPIService', PermissionAPIService);

})(appControllers);