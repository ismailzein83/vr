(function (appControllers) {

    'use strict';

    TenantService.$inject = ['VRModalService'];

    function TenantService(VRModalService) {
        return ({
            addTenant: addTenant,
            editTenant: editTenant
        });

        function addTenant(onTenantAdded) {
            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onTenantAdded = onTenantAdded;
            };

            VRModalService.showModal('/Client/Modules/Security/Views/Tenant/TenantEditor.html', null, modalSettings);
        }

        function editTenant(tenantId, onTenantUpdated) {
            var modalParameters = {
                tenantId: tenantId
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onTenantUpdated = onTenantUpdated;
            };

            VRModalService.showModal('/Client/Modules/Security/Views/Tenant/TenantEditor.html', modalParameters, modalSettings);
        }
    };

    appControllers.service('VR_Sec_TenantService', TenantService);

})(appControllers);
