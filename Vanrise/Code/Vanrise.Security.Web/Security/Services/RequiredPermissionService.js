(function (appControllers) {

    'use strict';

    RequiredPermissionService.$inject = ['VRModalService'];

    function RequiredPermissionService(VRModalService) {
        return ({
            addRequiredPermission: addRequiredPermission,
            editRequiredPermission: editRequiredPermission,
        });

        function addRequiredPermission(permissions, onSavePermmision) {
            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onSavePermmision = onSavePermmision;
            };
            var parameters = {
                Permissions: permissions
            };

            VRModalService.showModal('/Client/Modules/Security/Views/RequiredPermission/EditRequiredPermission.html', parameters, modalSettings);
        }

        function editRequiredPermission(requiredPermission, permissions, onSavePermmision) {
            var parameters = {
                RequiredPermission: requiredPermission,
                Permissions: permissions
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onSavePermmision = onSavePermmision;
            };

            VRModalService.showModal('/Client/Modules/Security/Views/RequiredPermission/EditRequiredPermission.html', parameters, modalSettings);
        }
    }

    appControllers.service('VR_Sec_RequiredPermissionService', RequiredPermissionService);

})(appControllers);
