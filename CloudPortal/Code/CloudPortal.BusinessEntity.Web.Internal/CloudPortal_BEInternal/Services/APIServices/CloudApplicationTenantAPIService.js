(function (appControllers) {

    "use strict";
    BEInternal_CloudApplicationTenantAPIService.$inject = ['BaseAPIService', 'UtilsService', 'CloudPortal_BEInternal_ModuleConfig','SecurityService'];

    function BEInternal_CloudApplicationTenantAPIService(BaseAPIService, UtilsService, CloudPortal_BEInternal_ModuleConfig, SecurityService) {
        var prefix = 'CloudApplicationTenant';

        function GetFilteredCloudApplicationTenants(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(CloudPortal_BEInternal_ModuleConfig.moduleName, prefix, "GetFilteredCloudApplicationTenants"), input);
        }

        function AssignCloudApplicationTenant(cloudApplicationTenant) {
            return BaseAPIService.post(UtilsService.getServiceURL(CloudPortal_BEInternal_ModuleConfig.moduleName, prefix, "AssignCloudApplicationTenant"), cloudApplicationTenant);
        }

        function GetCloudApplicationTenant(cloudApplicationTenantId) {
            return BaseAPIService.get(UtilsService.getServiceURL(CloudPortal_BEInternal_ModuleConfig.moduleName, prefix, "GetCloudApplicationTenant"), {
                cloudApplicationTenantId: cloudApplicationTenantId
            });
        }

        function UpdateCloudApplicationTenant(cloudApplicationTenant) {
            return BaseAPIService.post(UtilsService.getServiceURL(CloudPortal_BEInternal_ModuleConfig.moduleName, prefix, "UpdateCloudApplicationTenant"), cloudApplicationTenant);
        }

        function HasUpdateCloudApplicationTenantPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(CloudPortal_BEInternal_ModuleConfig.moduleName, prefix, ['UpdateCloudApplicationTenant']));
        }

        function HasAssignCloudApplicationTenantPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(CloudPortal_BEInternal_ModuleConfig.moduleName, prefix, ['AssignCloudApplicationTenant']));
        }

        return ({
            GetFilteredCloudApplicationTenants: GetFilteredCloudApplicationTenants,
            AssignCloudApplicationTenant: AssignCloudApplicationTenant,
            GetCloudApplicationTenant: GetCloudApplicationTenant,
            UpdateCloudApplicationTenant: UpdateCloudApplicationTenant,
            HasUpdateCloudApplicationTenantPermission: HasUpdateCloudApplicationTenantPermission,
            HasAssignCloudApplicationTenantPermission: HasAssignCloudApplicationTenantPermission
        });
    }

    appControllers.service('CloudPortal_BEInternal_CloudApplicationTenantAPIService', BEInternal_CloudApplicationTenantAPIService);

})(appControllers);