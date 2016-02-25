(function (appControllers) {

    'use strict';

    PermissionAPIService.$inject = ['BaseAPIService', 'VR_Sec_ModuleConfig', 'UtilsService', 'VR_Sec_SecurityAPIService'];

    function PermissionAPIService(BaseAPIService, VR_Sec_ModuleConfig, UtilsService, VR_Sec_SecurityAPIService) {

        return ({
            GetFilteredEntityPermissions: GetFilteredEntityPermissions,
            GetHolderPermissions: GetHolderPermissions,
            GetEffectivePermissions: GetEffectivePermissions,
            UpdatePermissions: UpdatePermissions,
            DeletePermissions: DeletePermissions,
            HasAddPermissionPermission: HasAddPermissionPermission,
            HasUpdatePermissionsPermission: HasUpdatePermissionsPermission
        });

        function GetFilteredEntityPermissions(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, 'Permission', 'GetFilteredEntityPermissions'), input);
        }

        function GetHolderPermissions(holderType, holderId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, 'Permission', 'GetHolderPermissions'), {
                holderType: holderType,
                holderId: holderId
            });
        }

        function GetEffectivePermissions() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, 'Permission', 'GetEffectivePermissions'));
        }

        function UpdatePermissions(permissionsArray) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, 'Permission', 'UpdatePermissions'), permissionsArray);
        }

        function DeletePermissions(holderType, holderId, entityType, entityId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, 'Permission', 'DeletePermissions'), {
                holderType: holderType,
                holderId: holderId,
                entityType: entityType,
                entityId: entityId
            });
        }

        function HasAddPermissionPermission() {
            return VR_Sec_SecurityAPIService.IsAllowed(VR_Sec_ModuleConfig.moduleName + '/Permission/UpdatePermissions');
        }

        function HasUpdatePermissionsPermission() {
            return VR_Sec_SecurityAPIService.IsAllowed(VR_Sec_ModuleConfig.moduleName + '/Permission/UpdatePermissions');
        }
    }

    appControllers.service('VR_Sec_PermissionAPIService', PermissionAPIService);

})(appControllers);