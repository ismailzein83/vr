(function (appControllers) {

    'use strict';

    PermissionService.$inject = ['VR_Sec_PermissionAPIService', 'VRModalService', 'VRNotificationService'];

    function PermissionService(VR_Sec_PermissionAPIService, VRModalService, VRNotificationService) {
        return {
            addPermission: addPermission,
            editPermissions: editPermissions,
            deletePermission: deletePermission,
            assignPermissions: assignPermissions
        };

        function addPermission(entityType, entityId, permissionOptions, permissions, onPermissionAdded) {
            var modalParameters = {
                entityType: entityType,
                entityId: entityId,
                permissionOptions: permissionOptions,
                permissions: permissions,
                notificationResponseText: 'Permissions'
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onPermissionAdded = onPermissionAdded;
            };

            VRModalService.showModal('/Client/Modules/Security/Views/Permission/BusinessEntityEditor.html', modalParameters, modalSettings);
        }

        function editPermissions(holderType, holderId, entityType, entityId, entityName, permissionFlags, permissionOptions, onPermissionsUpdated) {
            var modalParameters = {
                holderType: holderType,
                holderId: holderId,
                entityType: entityType,
                entityId: entityId,
                entityName: entityName,
                permissionFlags: permissionFlags,
                permissionOptions: permissionOptions,
                notificationResponseText: 'Permissions'
            };

            var modalSettings = {};
            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onPermissionsUpdated = onPermissionsUpdated;
            };

            VRModalService.showModal('/Client/Modules/Security/Views/Permission/BusinessEntityEditor.html', modalParameters, modalSettings);
        }

        function deletePermission(scope, dataItem, onPermissionDeleted) {
            VRNotificationService.showConfirmation().then(function (confirmed) {
                if (confirmed) {
                    return VR_Sec_PermissionAPIService.DeletePermissions(dataItem.Entity.HolderType, dataItem.Entity.HolderId, dataItem.Entity.EntityType, dataItem.Entity.EntityId).then(function (response) {
                        if (onPermissionDeleted && typeof onPermissionDeleted == 'function') {
                            onPermissionDeleted();
                        }
                    }).catch(function (error) {
                        VRNotificationService.notifyException(error, scope);
                    });
                }
            });
        }

        function assignPermissions(holderType, holderId) {
            var modalParameters = {
                holderType: holderType,
                holderId: holderId,
                notificationResponseText: 'Permissions'
            };

            var modalSettings = {};

            VRModalService.showModal('/Client/Modules/Security/Views/Permission/PermissionEditor.html', modalParameters, modalSettings);
        }
    }

    appControllers.service('VR_Sec_PermissionService', PermissionService);

})(appControllers);