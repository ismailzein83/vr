(function (appControllers) {

    "use strict";
    BEInternal_CloudApplicationTenantService.$inject = ['VRModalService'];
    function BEInternal_CloudApplicationTenantService(VRModalService) {

        function assignTenantToCloudApplication(cloudApplicationId, onTenantAssignedToCloudApplication) {
            var modalSettings = {
            };
            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onTenantAssignedToCloudApplication = onTenantAssignedToCloudApplication;
            };
            var parameters = {
                CloudApplicationId: cloudApplicationId
            };

            VRModalService.showModal('/Client/Modules/CloudPortal_BEInternal/Views/CloudApplicationTenant/CloudApplicationTenantEditor.html', parameters, modalSettings);
        }

        function editCloudApplicationTenant(cloudApplicationTenantId) {
            var parameters = {
                CloudApplicationTenantId: cloudApplicationTenantId
            };

            VRModalService.showModal('/Client/Modules/CloudPortal_BEInternal/Views/CloudApplicationTenant/CloudApplicationTenantEditor.html', parameters, undefined);
        }

        return ({
            assignTenantToCloudApplication: assignTenantToCloudApplication,
            editCloudApplicationTenant: editCloudApplicationTenant
        });
    }
    appControllers.service('CloudPortal_BEInternal_CloudApplicationTenantService', BEInternal_CloudApplicationTenantService);

})(appControllers);