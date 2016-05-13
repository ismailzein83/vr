(function (appControllers) {

    "use strict";
    BEInternal_CloudApplicationUserService.$inject = ['VRModalService'];
    function BEInternal_CloudApplicationUserService(VRModalService) {

        function assignUserToCloudApplication(cloudApplicationTenantId, tenantId, onUserAssignedToCloudApplication) {
            var modalSettings = {
            };
            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onUserAssignedToCloudApplication = onUserAssignedToCloudApplication;
            };
            var parameters = {
                CloudApplicationTenantId: cloudApplicationTenantId,
                TenantId: tenantId
            };

            VRModalService.showModal('/Client/Modules/CloudPortal_BEInternal/Views/CloudApplicationUser/CloudApplicationUserEditor.html', parameters, modalSettings);
        }

        return ({
            assignUserToCloudApplication: assignUserToCloudApplication
        });
    }
    appControllers.service('CloudPortal_BEInternal_CloudApplicationUserService', BEInternal_CloudApplicationUserService);

})(appControllers);