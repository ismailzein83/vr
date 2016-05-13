(function (appControllers) {

    "use strict";
    BEInternal_CloudApplicationAPIService.$inject = ['BaseAPIService', 'UtilsService', 'CloudPortal_BEInternal_ModuleConfig', 'SecurityService'];

    function BEInternal_CloudApplicationAPIService(BaseAPIService, UtilsService, CloudPortal_BEInternal_ModuleConfig, SecurityService) {
        var prefix = 'CloudApplication';

        function GetFilteredCloudApplications(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(CloudPortal_BEInternal_ModuleConfig.moduleName, prefix, "GetFilteredCloudApplications"), input);
        }

        function GetCloudApplication(id) {
            return BaseAPIService.get(UtilsService.getServiceURL(CloudPortal_BEInternal_ModuleConfig.moduleName, prefix, "GetCloudApplication"), {
                id: id
            });
        }

        function AddCloudApplication(cloudApplication) {
            return BaseAPIService.post(UtilsService.getServiceURL(CloudPortal_BEInternal_ModuleConfig.moduleName, prefix, "AddCloudApplication"), cloudApplication);
        }

        function UpdateCloudApplication(cloudApplication) {
            return BaseAPIService.post(UtilsService.getServiceURL(CloudPortal_BEInternal_ModuleConfig.moduleName, prefix, "UpdateCloudApplication"), cloudApplication);
        }

        function GetCloudApplicationByUser() {
            return BaseAPIService.get(UtilsService.getServiceURL(CloudPortal_BEInternal_ModuleConfig.moduleName, prefix, "GetCloudApplicationByUser"));
        }


        function HasUpdateCloudApplicationPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(CloudPortal_BEInternal_ModuleConfig.moduleName, prefix, ['UpdateCloudApplication']));
        }

        function HasAddCloudApplicationPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(CloudPortal_BEInternal_ModuleConfig.moduleName, prefix, ['AddCloudApplication']));
        }

        return ({
            GetFilteredCloudApplications: GetFilteredCloudApplications,
            GetCloudApplication: GetCloudApplication,
            AddCloudApplication: AddCloudApplication,
            UpdateCloudApplication: UpdateCloudApplication,
            GetCloudApplicationByUser: GetCloudApplicationByUser,
            HasUpdateCloudApplicationPermission: HasUpdateCloudApplicationPermission,
            HasAddCloudApplicationPermission: HasAddCloudApplicationPermission
        });
    }

    appControllers.service('CloudPortal_BEInternal_CloudApplicationAPIService', BEInternal_CloudApplicationAPIService);

})(appControllers);