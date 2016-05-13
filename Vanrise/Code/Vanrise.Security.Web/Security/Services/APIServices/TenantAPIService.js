(function (appControllers) {

    'use strict';

    TenAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_Sec_ModuleConfig', 'SecurityService'];

    function TenAPIService(BaseAPIService, UtilsService, VR_Sec_ModuleConfig, SecurityService) {
        var controllerName = 'Tenants';

        return ({
            GetFilteredTenants: GetFilteredTenants,
            GetTenantsInfo: GetTenantsInfo,
            GetTenantbyId: GetTenantbyId,
            AddTenant: AddTenant,
            UpdateTenant: UpdateTenant,
            HasAddTenantPermission: HasAddTenantPermission,
            HasUpdateTenantPermission: HasUpdateTenantPermission
        });

        function GetFilteredTenants(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, controllerName, 'GetFilteredTenants'), input);
        }

        function GetTenantsInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, controllerName, 'GetTenantsInfo'), {
                filter: filter
            });
        }

        function GetTenantbyId(tenantId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, controllerName, 'GetTenantbyId'), {
                tenantId: tenantId
            });
        }

        function AddTenant(tenant) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, controllerName, 'AddTenant'), tenant);
        }

        function UpdateTenant(tenant) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, controllerName, 'UpdateTenant'), tenant);
        }

        function HasAddTenantPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(VR_Sec_ModuleConfig.moduleName, controllerName, ['AddTenant']));
        }

        function HasUpdateTenantPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(VR_Sec_ModuleConfig.moduleName, controllerName, ['UpdateTenant']));
        }
    }

    appControllers.service('VR_Sec_TenantAPIService', TenAPIService);

})(appControllers);