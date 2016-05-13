(function (appControllers) {

    "use strict";
    BEInternal_CloudApplicationUserAPIService.$inject = ['BaseAPIService', 'UtilsService', 'CloudPortal_BEInternal_ModuleConfig', 'SecurityService'];

    function BEInternal_CloudApplicationUserAPIService(BaseAPIService, UtilsService, CloudPortal_BEInternal_ModuleConfig, SecurityService) {
        var prefix = 'CloudApplicationUser';

        function GetFilteredCloudApplicationUsers(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(CloudPortal_BEInternal_ModuleConfig.moduleName, prefix, "GetFilteredCloudApplicationUsers"), input);
        }

        function AssignCloudApplicationUser(cloudApplicationUser) {
            return BaseAPIService.post(UtilsService.getServiceURL(CloudPortal_BEInternal_ModuleConfig.moduleName, prefix, "AssignCloudApplicationUser"), cloudApplicationUser);
        }

        function AssignCloudApplicationUserWithPermission(cloudApplicationUser) {
            return BaseAPIService.post(UtilsService.getServiceURL(CloudPortal_BEInternal_ModuleConfig.moduleName, prefix, "AssignCloudApplicationUserWithPermission"), cloudApplicationUser);
        }

        function HasAssignCloudApplicationUserPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(CloudPortal_BEInternal_ModuleConfig.moduleName, prefix, ['AssignCloudApplicationUser', 'AssignCloudApplicationUserWithPermission']));
        }

        return ({
            GetFilteredCloudApplicationUsers: GetFilteredCloudApplicationUsers,
            AssignCloudApplicationUser: AssignCloudApplicationUser,
            AssignCloudApplicationUserWithPermission: AssignCloudApplicationUserWithPermission,
            HasAssignCloudApplicationUserPermission: HasAssignCloudApplicationUserPermission
        });
    }

    appControllers.service('CloudPortal_BEInternal_CloudApplicationUserAPIService', BEInternal_CloudApplicationUserAPIService);

})(appControllers);